using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Codeer.Friendly.Windows;
using System.Diagnostics;
using Codeer.Friendly.Windows.Grasp;
using Codeer.Friendly;

namespace FriendlyBaseTest
{
    /// <summary>
    /// 対象プロセスのCLRバージョンに合わせて動作する検査
    /// </summary>
    [TestFixture]
    public class TestTargetClrVerFull
    {
        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUp()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path32, "v2.0.50727"));
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path64, "v2.0.50727"));
            }
        }

        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUpWpf()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf32, "v4.0.30319"));
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf64, "v4.0.30319"));
            }
        }

        /// <summary>
        /// 2.0
        /// </summary>
        [Test]
        public void Test20()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl top = WindowControl.FromZTop(app);
                Assert.AreEqual(top["Text"]().ToString(), "TargetForm");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 4.0
        /// </summary>
        [Test]
        public void Test40()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUpWpf())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl top = WindowControl.FromZTop(app);
                Assert.AreEqual(top["Title"]().ToString(), "MainWindow");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }
    }
}


