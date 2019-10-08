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
    public class TestTargetClrVerForecast
    {
        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUp()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path32));
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path64));
            }
        }

        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUpWpf()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf32));
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf64));
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

        /// <summary>
        /// CLRなし
        /// </summary>
        [Test]
        public void TestNative()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            int id = 0;
            try
            {
                using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc)))
                {
                    id = app.ProcessId;
                    WindowControl top = WindowControl.FromZTop(app);
                    Assert.AreEqual(top.GetWindowText(), "MfcTestTarget");
                }
            }
            finally
            {
                Process.GetProcessById(id).CloseMainWindow();
            }
        }

        /// <summary>
        /// CLRの予測が不能な場合のテスト
        /// </summary>
        [Test]
        public void TestErrorCLRForecast()
        {
            Process process = null;
            try
            {
                if (IntPtr.Size == 4)
                {
                    process = Process.Start(TargetPath.Path32);
                }
                else
                {
                    process = Process.Start(TargetPath.Path64);
                }
                using (WindowsAppFriend app = new WindowsAppFriend(process, "2.0")) { }
                using (WindowsAppFriend app = new WindowsAppFriend(process, "4.0")) { }
                WindowsAppFriend appCpuTest = new WindowsAppFriend(process);
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual(e.Message, "CLRのバージョンが予測できません。" + Environment.NewLine +
                    "対象アプリ内に複数のCLRがロードされている可能性があります。" + Environment.NewLine +
                     "対象のCLRのバージョンを明示するコンストラクタを使用してください。");
            }
            finally
            {
                process.Kill();
            }
        }
    }
}


