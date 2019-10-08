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
using System.Runtime.InteropServices;

namespace FriendlyBaseTest
{
    /// <summary>
    /// TestWindowsAppFriendInterfaceのインターフェイスに対する検査
    /// </summary>
    [TestFixture]
    public class TestWindowsAppFriendAndExecuteContextInterface
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
        /// ExecuteContextを破棄した後通信できなくなるテスト
        /// </summary>
        [Test]
        public void TestInvalidContext()
        {
            WindowsAppFriend app2 = null;
            if (IntPtr.Size == 4)
            {
                app2 = new WindowsAppFriend(Process.Start(TargetPath.Path32), "2.0");
            }
            else
            {
                app2 = new WindowsAppFriend(Process.Start(TargetPath.Path64), "2.0");
            }

            try
            {
                WindowControl targetForm = WindowControl.FromZTop(app2);
                //実行コンテキストを変更
                ExecuteContext context = new ExecuteContext(app2, targetForm.AppVar);
                app2.ChangeContext(context);
                //破棄
                context.Dispose();

                //通信に失敗すること
                try
                {
                    app2.Dim();
                    Assert.IsTrue(false);
                }
                catch (FriendlyOperationException e)
                {
                    string message = "アプリケーションとの通信に失敗しました。" + Environment.NewLine +
                                        "対象アプリケーションが通信不能な状態になったか、" + Environment.NewLine +
                                        "シリアライズ不可能な型のデータを転送しようとした可能性があります。";
                    Assert.AreEqual(e.Message, message);
                }
            }
            finally
            {
                Process.GetProcessById(app2.ProcessId).CloseMainWindow();
            }
        }

        /// <summary>
        /// 切り替えたExecuteContextがGCによって破棄されないこと
        /// </summary>
        [Test]
        public void TestContextNotDisposedByGC()
        {
            WindowsAppFriend app2 = null;
            if (IntPtr.Size == 4)
            {
                app2 = new WindowsAppFriend(Process.Start(TargetPath.Path32), "2.0");
            }
            else
            {
                app2 = new WindowsAppFriend(Process.Start(TargetPath.Path64), "2.0");
            }

            try
            {
                WindowControl targetForm = WindowControl.FromZTop(app2);
                //実行コンテキストを変更
                ExecuteContext context = new ExecuteContext(app2, targetForm.AppVar);
                var old = app2.ChangeContext(context);
                //古いコンテキストを破棄
                old.Dispose();

                //nullにしてもapp2が持っているのでコンテキストは破棄されない
                context = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //これは正常に動くこと
                app2.Dim();
            }
            finally
            {
                Process.GetProcessById(app2.ProcessId).CloseMainWindow();
            }
        }
        
        /// <summary>
        /// スレッドコンテキストの切り替えテスト
        /// ついでにExecuteContextのDisposeのテスト
        /// </summary>
        [Test]
        public void TestChangeContextByWindowObject()
        {
            //フォームの取得
            WindowControl targetForm = WindowControl.FromZTop(app);
            
            //別スレッドウィンドウを表示
            WindowControl _buttonOtherThread = targetForm.IdentifyFromZIndex(0);
            _buttonOtherThread["PerformClick"]();

            //表示待ち
            AppVar next = null;
            while (next == null)
            {
                Thread.Sleep(10);
                AppVar all = app["System.Windows.Forms.Application.OpenForms"]();
                foreach (AppVar element in new Enumerate(all))
                {
                    if (!(bool)targetForm.AppVar["Equals"](element).Core)
                    {
                        next = element;
                        break;
                    }
                }
            }

            //実行コンテキストを変更
            using (ExecuteContext context = new ExecuteContext(app, next))
            {
                ExecuteContext oldContext = app.ChangeContext(context);
                AppVar _textBoxSrc = next["_textBoxSrc"]();
                _textBoxSrc["Text"]("xxx");
                AppVar _buttonCopyText = next["_buttonCopyText"]();
                _buttonCopyText["PerformClick"]();
                AppVar _textBoxDst = next["_textBoxDst"]();
                //操作の結果_textBoxDstにxxxが入力される
                Assert.AreEqual(_textBoxDst["Text"]().ToString(), "xxx");
                try
                {
                    next["Close"]();
                }
                catch (FriendlyOperationException) { }//スレッドが終了してしまうので、通信途中で通信不能となる　これは仕様

                //コンテキストをもとに戻す
                app.ChangeContext(oldContext);
            }
            //ウィンドウを操作しても例外が発生しない
            targetForm["Handle"]();
            targetForm["Text"]("xxx");
        }

        /// <summary>
        /// ハンドルによるスレッドコンテキストの切り替えテスト
        /// ついでに、app.ProccessId
        /// </summary>
        [Test]
        public void TestChangeContextByWindowHandle()
        {
            //フォームの取得
            WindowControl targetForm = WindowControl.FromZTop(app);
            
            //別スレッドウィンドウを表示
            WindowControl _buttonOtherThread = targetForm.IdentifyFromZIndex(0);
            _buttonOtherThread["PerformClick"]();

            //表示待ち
            IntPtr nextHandle = IntPtr.Zero;
            while (nextHandle == IntPtr.Zero)
            {
                EnumWindowsDelegate callBack = delegate(IntPtr hWnd, int lParam)
                {
                    int lpdwProcessId;
                    GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                    if (lpdwProcessId != app.ProcessId)
                    {
                        return 1;
                    }

                    StringBuilder lpString = new StringBuilder(1000);
                    GetWindowText(hWnd, lpString, 1000);
                    if (lpString.ToString() == "NormalGuiForm")
                    {
                        nextHandle = hWnd;
                        return 0;
                    }
                    return 1;
                };
                EnumWindows(callBack, 0);
                GC.KeepAlive(callBack);
            }

            //実行コンテキストを変更
            using (ExecuteContext context = new ExecuteContext(app, nextHandle))
            {
                ExecuteContext oldContext = app.ChangeContext(context);

                //ウィンドウコントロール生成
                WindowControl next = new WindowControl(app, nextHandle);

                //別スレッドウィンドウを操作
                AppVar _textBoxSrc = next["_textBoxSrc"]();
                _textBoxSrc["Text"]("xxx");
                AppVar _buttonCopyText = next["_buttonCopyText"]();
                _buttonCopyText["PerformClick"]();
                AppVar _textBoxDst = next["_textBoxDst"]();
                //操作の結果_textBoxDstにxxxが入力される
                Assert.AreEqual(_textBoxDst["Text"]().ToString(), "xxx");
                try
                {
                    next["Close"]();
                }
                catch (FriendlyOperationException) { }//スレッドが終了してしまうので、通信途中で通信不能となる　これは仕様

                //コンテキストをもとに戻す
                app.ChangeContext(oldContext);
            }
            //ウィンドウを操作しても例外が発生しない
            targetForm["Handle"]();
            targetForm["Text"]("xxx");
        }

        /// <summary>
        /// 二重起動正常系とWindowsAppFriendのハンドルからの生成テスト
        /// </summary>
        [Test]
        public void TestAppConstructorByHandleAndDoubleAppFriendAccess()
        {
            //ウィンドウハンドルからWindowsAppFriendを生成
            using (WindowsAppFriend app2 = new WindowsAppFriend(Process.GetProcessById(app.ProcessId).MainWindowHandle, "2.0"))
            {
                //二重起動した方でテキストの変更
                WindowControl targetForm = WindowControl.FromZTop(app2);
                targetForm["Text"]("xxx");
            }

            //app2をDispose後最初の方でテキストを取得
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                Assert.AreEqual(targetForm["Text"]().ToString(), "xxx");
            }
        }

        /// <summary>
        /// 二重起動操作例外発生
        /// </summary>
        [Test]
        public void TestDoubleAppFriendAccessException()
        {
            //ウィンドウハンドルからWindowsAppFriendを生成
            using (WindowsAppFriend app2 = new WindowsAppFriend(Process.GetProcessById(app.ProcessId).MainWindowHandle, "2.0"))
            {
                AppVar vInApp = app.Dim(3);
                AppVar vInApp2 = app2.Dim(3);
                try
                {
                    vInApp["Equals"](vInApp2);
                    Assert.IsTrue(false);
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual(e.Message, "第1引数が不正です。" + Environment.NewLine + 
                        "引数に使用されたAppVarの中に、異なるAppFriendの管理する変数プールに属するAppVarがあります。");
                }
            }
        }

        /// <summary>
        /// アプリケーションコントロール情報テスト
        /// </summary>
        [Test]
        public void TestAppControlInfo()
        {
            app.AddAppControlInfo("xxx", "yyy");
            object obj;
            Assert.IsFalse(app.TryGetAppControlInfo("zzz", out obj));
            Assert.IsTrue(app.TryGetAppControlInfo("xxx", out obj));
            Assert.IsTrue((string)obj == "yyy");
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

        /// <summary>
        /// ウィンドウ列挙時のハンドラ
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lParam">パラメータ</param>
        /// <returns>0を返すと列挙終了</returns>
        internal delegate int EnumWindowsDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// ウィンドウ列挙
        /// </summary>
        /// <param name="lpEnumFunc">列挙コールバック</param>
        /// <param name="lParam">パラメータ</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll")]
        internal static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, int lParam);

        /// <summary>
        /// 指定のウィンドウハンドルの所属するスレッドとプロセスの取得
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpdwProcessId">プロセスID</param>
        /// <returns>スレッドID</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        /// Window文字列取得
        /// </summary>
        /// <param name="hWnd">ハンドル</param>
        /// <param name="lpString">文字列格納バッファ</param>
        /// <param name="nMaxCount">最大文字列</param>
        /// <returns>結果</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
