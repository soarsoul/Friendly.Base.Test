using Codeer.Friendly.Windows;
using NUnit.Framework;
using System.Diagnostics;
using System;
using Codeer.Friendly.Windows.Grasp;
using Codeer.Friendly;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FriendlyBaseTest
{
    /// <summary>
    /// 通信の不安点のテスト
    /// </summary>
    [TestFixture]
    public class TestCommunicationWeak
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
        /// PostMessageでキューがあふれたときのテスト
        /// </summary>
        [Test]
        public void TestMaxPostMessage1()
        {
            WindowControl targetForm = WindowControl.FromZTop(app);
            AppVar _textBox = targetForm["_testUserControl"]()["_textBox"]();
            targetForm.SendMessage(0x8005, IntPtr.Zero, IntPtr.Zero, new Async());
            Thread.Sleep(1000);
            _textBox["Text"]("aaa");
            Assert.AreEqual(_textBox["Text"](), "aaa");
        }

        /// <summary>
        /// PostMessageでキューがあふれたときのテスト
        /// </summary>
        [Test]
        public void TestMaxPostMessage2()
        {
            WindowControl targetForm = WindowControl.FromZTop(app);
            app[typeof(Thread), "Sleep", new Async()](20000);
            targetForm["_incValue"](0);
            for (int i = 0; i < 11000; i++)
            {
                targetForm["Inc", new Async()]();
            }
            Assert.AreEqual(targetForm["_incValue"](), 11000);
        }

        class WaitForm : Form
        {
            WindowsAppFriend _app;
            WindowControl _oldTop;
            internal WaitForm(WindowsAppFriend app, WindowControl oldTop)
            {
                _app = app;
                _oldTop = oldTop;
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x8001)
                {
                    WindowControl v = WindowControl.FromZTop(_app);
                    m.Result = new IntPtr((_oldTop.Handle == v.Handle) ? 0 : 1);
                    return;
                }
                else if (m.Msg == 0x8002)
                {
                    WindowControl top = _oldTop.WaitForNextModal();
                    WindowControl c = top.IdentifyFromWindowText("OK");
                    c.SetFocus();
                    c.SequentialMessage(
                        new MessageInfo(0x0201, 0x0001, 0),
                        new MessageInfo(0x0202, 0x0000, 0));
                }
                base.WndProc(ref m);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 通信番号テスト
        /// 通信が入れ子になる
        /// </summary>
        [Test]
        public void TestCommunicationNo()
        {
            WaitForm w = new WaitForm(app, WindowControl.FromZTop(app));
            IntPtr h = w.Handle;
            Thread t = new Thread((ThreadStart)delegate
            {
                while (SendMessage(h, 0x8001, IntPtr.Zero, IntPtr.Zero).ToInt32() == 0)
                {
                    Thread.Sleep(10);
                }
                SendMessage(h, 0x8002, IntPtr.Zero, IntPtr.Zero);
            });
            t.Start();
            app[typeof(MessageBox), "Show"]("");
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
