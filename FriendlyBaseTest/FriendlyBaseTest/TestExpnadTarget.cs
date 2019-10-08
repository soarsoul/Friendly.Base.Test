using System;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using NUnit.Framework;
using FriendlyBaseTargetNet20;
using ExpandTestTarget;

namespace FriendlyBaseTest
{
    /// <summary>
    /// アプリケーション拡張のテスト
    /// </summary>
    [TestFixture]
    public class TestExpnadTarget
    {
        WindowsAppFriend app;

        /// <summary>
        /// 用意
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (IntPtr.Size == 4)
            {
                app = new WindowsAppFriend(Process.Start(TargetPath.Path32), "2.0");
            }
            else
            {
                app = new WindowsAppFriend(Process.Start(TargetPath.Path64), "2.0");
            }
        }

        /// <summary>
        /// ファイル名称からアセンブリのロードテスト
        /// </summary>
        [Test]
        public void TestExpandAssemblyFile()
        {
            WindowControl targetForm = WindowControl.FromZTop(app);
            WindowsAppExpander.LoadAssemblyFromFile(app, typeof(ExpandMethods).Assembly.Location);
            app[typeof(ExpandMethods), "SetText"]("xxx", targetForm.AppVar);
            Assert.AreEqual(targetForm["Text"]().ToString(), "xxx");
            Assert.AreEqual((int)app[typeof(ExpandMethods), "GetDataValue"](new Data(3)).Core, 3);
        }
        
        /// <summary>
        /// ファイル名称からアセンブリのロードテスト
        /// ２回目呼んでも問題ないことの確認
        /// </summary>
        [Test]
        public void TestExpandAssemblyFile2()
        {
            TestExpandAssemblyFile();
        }

        /// <summary>
        /// アセンブリをフルネームからロード
        /// </summary>
    //    [Test]
        public void TestExpandAssemblyFullName()
        {
            //NUnitのランタイムが4.0になったので一旦このテストは保留
            WindowControl targetForm = WindowControl.FromZTop(app);
            WindowsAppExpander.LoadAssemblyFromFullName(app, typeof(Microsoft.VisualBasic.Interaction).Assembly.FullName);
            app[typeof(Microsoft.VisualBasic.Interaction), "AppActivate"](app.ProcessId);
        }
                
        /// <summary>
        /// アセンブリをフルネームからロード
        /// ２回目呼んでも問題ないことの確認
        /// </summary>
    //    [Test]
        public void TestExpandAssemblyFullName2()
        {
            TestExpandAssemblyFullName();
        }

        /// <summary>
        /// DLLのロード
        /// </summary>
        [Test]
        public void TestLoadLibrary()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }

            Assert.IsTrue(WindowsAppExpander.LoadLibrary(app, TargetPath.PathExpandNative));
            WindowsAppExpander.LoadAssemblyFromFile(app, typeof(ExpandMethods).Assembly.Location);

            //ロードしたDLLからMFCのメッセージボックスを呼び出し、それを閉じる
            WindowControl current = WindowControl.FromZTop(app);
            app[typeof(ExpandMethods), "Func", new Async()]("xxx");
            WindowControl next = current.WaitForNextModal();
            WindowControl button2 = next.IdentifyFromZIndex(2);
            button2.SetFocus();
            button2.SequentialMessage(
                new MessageInfo(0x0201, 0x0001, 0),
                new MessageInfo(0x0202, 0x0000, 0));
        }
                
        /// <summary>
        /// DLLのロード
        /// ２回目呼んでも問題ないことの確認
        /// </summary>
        [Test]
        public void TestLoadLibrary2()
        {
            TestLoadLibrary();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (app != null)
            {
                app.Dispose();
                Process process = Process.GetProcessById(app.ProcessId);
                process.CloseMainWindow();
                app = null;
            }
        }
    }
}

