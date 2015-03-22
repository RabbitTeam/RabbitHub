using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.IO;
using System.Threading;

namespace Rabbit.Kernel.Tests.FileSystems.VirtualPath
{
    [TestClass]
    public sealed class DefaultVirtualPathMonitorTests : TestBase
    {
        #region Property

        public IVirtualPathProvider VirtualPathProvider { get; set; }

        public IVirtualPathMonitor VirtualPathMonitor { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void WhenPathChangesTest()
        {
            //监控文件
            WhenPathChanges("~/c/a.txt", VirtualPathProvider.FileExists, s => VirtualPathProvider.CreateFile(s, stream => { }), VirtualPathProvider.DeleteFile,
                s =>
                {
                    var path = VirtualPathProvider.MapPath(s);
                    File.WriteAllText(path, "Test");
                });
            //监控目录
            WhenPathChanges("~/c/a", VirtualPathProvider.DirectoryExists, VirtualPathProvider.CreateDirectory, VirtualPathProvider.DeleteDirectory, null);

            if (VirtualPathProvider.DirectoryExists("~/c"))
                VirtualPathProvider.DeleteDirectory("~/c");
        }

        #endregion Test Method

        #region Private Method

        private void WhenPathChanges(string path, Func<string, bool> exists, Action<string> create, Action<string> delete, Action<string> modify)
        {
            if (exists(path))
                delete(path);

            var token = VirtualPathMonitor.WhenPathChanges(path);
            //验证方法。
            Action<bool> val = b =>
            {
                if (b)
                    Assert.IsTrue(token.IsCurrent);
                else
                    Assert.IsFalse(token.IsCurrent);
            };

            //第一次监控始终为true
            val(true);
            val(true);

            //创建文件，文件状态变更（之前不存在）
            create(path);
            Thread.Sleep(1000);
            //监控状态应为false
            val(false);
            /*//之后的状态都为true
            val(true);
            val(true);*/

            if (modify != null)
            {
                //写入文件，文件状态变更（内容变更）
                modify(VirtualPathProvider.MapPath(path));
                Thread.Sleep(1000);
                /*VirtualPathProvider.OpenFile(path, stream =>
                {
                    using (var writer = new StreamWriter(stream))
                        writer.Write("Test");
                });*/
                //监控状态应为false
                val(false);
                /*//之后的状态都为true
                val(true);
                val(true);*/
            }

            //删除文件，文件状态变更（之前存在）
            delete(path);
            Thread.Sleep(1000);

            //监控状态应为false
            val(false);
            /*            //之后的状态都为true
                        val(true);
                        val(true);*/
        }

        #endregion Private Method
    }
}