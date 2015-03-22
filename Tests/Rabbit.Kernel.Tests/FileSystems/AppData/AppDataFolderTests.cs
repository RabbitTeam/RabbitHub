using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.IO;

namespace Rabbit.Kernel.Tests.FileSystems.AppData
{
    [TestClass]
    public class AppDataFolderTests : TestBase, IDisposable
    {
        public IAppDataFolder AppDataFolder { get; set; }

        public IVirtualPathProvider VirtualPathProvider { get; set; }

        [TestMethod]
        public void MapPathTest()
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            Assert.AreEqual(baseDirectory, AppDataFolder.MapPath("~/"));
            Assert.AreEqual(Path.Combine(baseDirectory, "a", "c.txt"), AppDataFolder.MapPath("~/a/b/../c.txt"));

            Assert.AreEqual(baseDirectory, AppDataFolder.MapPath(AppDataFolder.MapPath("~/")));
            Assert.AreEqual(Path.Combine(baseDirectory, "a"), AppDataFolder.MapPath(AppDataFolder.MapPath("~/a")));

            baseDirectory += "\\";
            Assert.AreEqual(baseDirectory, AppDataFolder.MapPath(baseDirectory));
        }

        [TestMethod]
        public void CreateFileAndReadFileTest()
        {
            const string path = "~/b/a.txt";
            const string text = "Test";

            AppDataFolder.CreateFile(path, text);
            Assert.AreEqual(text, AppDataFolder.ReadFile(path));
        }

        [TestMethod]
        public void StoreFileTest()
        {
            const string path1 = "~/b/b.txt";
            const string path2 = "~/b/c.txt";
            const string text = "Test";

            AppDataFolder.CreateFile(path1, text);
            AppDataFolder.StoreFile(AppDataFolder.MapPath(path1), path2);
            Assert.AreEqual(text, AppDataFolder.ReadFile(path2));

            AppDataFolder.DeleteFile(path1);
            AppDataFolder.DeleteFile(path2);
        }

        [TestMethod]
        public void GetVirtualPathTest()
        {
            const string path = "~/b/a.txt";
            var fullPath = AppDataFolder.MapPath(path);
            Assert.AreEqual("~/App_Data/b/a.txt", AppDataFolder.GetVirtualPath(fullPath));
            Assert.AreEqual("~/App_Data/b/b.txt", AppDataFolder.GetVirtualPath(VirtualPathProvider.MapPath("~/b/b.txt")));
            Assert.AreEqual("~/App_Data/a/b/c", AppDataFolder.GetVirtualPath("~/a/b/c"));
            Assert.AreEqual("~/App_Data/a/b/c", AppDataFolder.GetVirtualPath("a/b/c"));
            Assert.AreEqual("~/App_Data/a/b/c", AppDataFolder.GetVirtualPath("/a/b/c"));
        }

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            try { VirtualPathProvider.DeleteDirectory("~/App_Data"); }
            catch { }
        }

        #endregion Implementation of IDisposable
    }
}