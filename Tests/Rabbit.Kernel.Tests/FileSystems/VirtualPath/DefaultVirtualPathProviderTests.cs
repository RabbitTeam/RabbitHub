using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Rabbit.Kernel.Tests.FileSystems.VirtualPath
{
    [TestClass]
    public class DefaultVirtualPathProviderTests : TestBase, IDisposable
    {
        public DefaultVirtualPathProviderTests()
        {
            VirtualPathProvider.CreateFile("~/a.txt", s => { });
            VirtualPathProvider.CreateFile("~/a/a.txt", s => { });
            VirtualPathProvider.CreateFile("~/a/b/a.txt", s => { });
        }

        #region Property

        public IVirtualPathProvider VirtualPathProvider { get; set; }

        #endregion Property

        [TestMethod]
        public void TryFileExistsTest()
        {
            Assert.IsTrue(VirtualPathProvider.FileExists("~/a.txt"));
            Assert.IsFalse(VirtualPathProvider.FileExists("~/../a.txt"));
            Assert.IsTrue(VirtualPathProvider.FileExists("~/a/../a.txt"));
            Assert.IsTrue(VirtualPathProvider.FileExists("~/a/b/../a.txt"));
            Assert.IsTrue(VirtualPathProvider.FileExists("~/a/b/../../a.txt"));
            Assert.IsFalse(VirtualPathProvider.FileExists("~/a/b/../../../a.txt"));
            Assert.IsFalse(VirtualPathProvider.FileExists("~/a/../../b/c.txt"));
        }

        [TestMethod]
        public void CombineTest()
        {
            Assert.AreEqual("a/b/c/d", VirtualPathProvider.Combine("a", "b", "c", "d"));
        }

        [TestMethod]
        public void MapPathTest()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Assert.AreEqual(baseDirectory, VirtualPathProvider.MapPath("~/"));
            Assert.AreEqual(baseDirectory, VirtualPathProvider.MapPath(""));
            Assert.AreEqual(baseDirectory, VirtualPathProvider.MapPath("/"));
            Assert.AreEqual(Path.Combine(baseDirectory, "a", "b"), VirtualPathProvider.MapPath("~/a/b"));
            Assert.AreEqual(Path.Combine(baseDirectory, "a", "b"), VirtualPathProvider.MapPath("/a/b"));
        }

        [TestMethod]
        public void OpenFileTest()
        {
            Stream globalStream = null;
            VirtualPathProvider.OpenFile("~/a/a.txt", stream =>
            {
                globalStream = stream;
            });
            Assert.IsFalse(globalStream.CanRead);
        }

        [TestMethod]
        public void CreateTextTest()
        {
            const string filePath = "~/a/b.txt";
            const string testText = "Test";

            if (VirtualPathProvider.FileExists(filePath))
                VirtualPathProvider.DeleteFile(filePath);

            VirtualPathProvider.CreateText(filePath, writer => writer.Write(testText));

            var text = VirtualPathProvider.OpenFileFunc(filePath, stream =>
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(bytes);
            });

            Assert.AreEqual(testText, text);

            VirtualPathProvider.DeleteFile(filePath);
        }

        [TestMethod]
        public void CreateFileTest()
        {
            const string filePath = "~/a/c.txt";

            if (VirtualPathProvider.FileExists(filePath))
                VirtualPathProvider.DeleteFile(filePath);

            VirtualPathProvider.CreateFile(filePath, stream => { });

            Assert.IsTrue(VirtualPathProvider.FileExists(filePath));

            VirtualPathProvider.DeleteFile(filePath);
        }

        [TestMethod]
        public void GetFileLastWriteTimeUtcTest()
        {
            const string filePath = "~/a/d.txt";

            if (VirtualPathProvider.FileExists(filePath))
                VirtualPathProvider.DeleteFile(filePath);

            VirtualPathProvider.CreateFile(filePath, stream => { });
            var date = VirtualPathProvider.GetFileLastWriteTimeUtc(filePath);
            Assert.IsNotNull(date);
            Assert.AreEqual(DateTime.UtcNow.ToLongDateString(), date.Value.ToLongDateString());

            VirtualPathProvider.DeleteFile(filePath);
        }

        [TestMethod]
        public void GetDirectoryLastWriteTimeUtcTest()
        {
            const string path = "~/a/c";

            if (VirtualPathProvider.DirectoryExists(path))
                VirtualPathProvider.DeleteDirectory(path);

            VirtualPathProvider.CreateDirectory(path);
            var date = VirtualPathProvider.GetDirectoryLastWriteTimeUtc(path);
            Assert.IsNotNull(date);
            Assert.AreEqual(DateTime.UtcNow.ToLongDateString(), date.Value.ToLongDateString());

            VirtualPathProvider.DeleteDirectory(path);
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            const string filePath = "~/a/e.txt";

            if (!VirtualPathProvider.FileExists(filePath))
                VirtualPathProvider.CreateFile(filePath, stream => { });

            VirtualPathProvider.DeleteFile(filePath);

            Assert.IsFalse(VirtualPathProvider.FileExists(filePath));
        }

        [TestMethod]
        public void DirectoryExistsTest()
        {
            const string path = "~/a/c";

            Assert.IsFalse(VirtualPathProvider.DirectoryExists(path));

            VirtualPathProvider.CreateDirectory(path);

            Assert.IsTrue(VirtualPathProvider.DirectoryExists(path));

            VirtualPathProvider.DeleteDirectory(path);
        }

        [TestMethod]
        public void CreateDirectoryTest()
        {
            const string path = "~/a/d";

            if (!VirtualPathProvider.DirectoryExists(path))
                VirtualPathProvider.DeleteDirectory(path);

            VirtualPathProvider.CreateDirectory(path);

            Assert.IsTrue(VirtualPathProvider.DirectoryExists(path));

            VirtualPathProvider.DeleteDirectory(path);
        }

        [TestMethod]
        public void GetDirectoryNameTest()
        {
            var name = VirtualPathProvider.GetDirectoryName("~/a/b/c");
            Assert.AreEqual("~/a/b", name);
        }

        [TestMethod]
        public void DeleteDirectoryTest()
        {
            const string path = "~/a/e";

            if (!VirtualPathProvider.DirectoryExists(path))
                VirtualPathProvider.CreateDirectory(path);

            VirtualPathProvider.DeleteDirectory(path);

            Assert.IsFalse(VirtualPathProvider.DirectoryExists(path));
        }

        [TestMethod]
        public void ListFilesTest()
        {
            var list = VirtualPathProvider.ListFiles("~/a");
            Assert.AreEqual(1, list.Count());
            list = VirtualPathProvider.ListFiles("~/a", true);
            Assert.AreEqual(2, list.Count());
        }

        [TestMethod]
        public void ListDirectoriesTest()
        {
            var list = VirtualPathProvider.ListDirectories("~/a");
            Assert.AreEqual(1, list.Count());
            list = VirtualPathProvider.ListDirectories("~/a", true);
            Assert.AreEqual(1, list.Count());
        }

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            VirtualPathProvider.DeleteDirectory("~/a");
            VirtualPathProvider.DeleteFile("~/a.txt");
        }

        #endregion Implementation of IDisposable
    }
}