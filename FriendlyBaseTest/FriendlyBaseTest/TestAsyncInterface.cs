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
    /// Asyncのインターフェイスをテスト
    /// 詳細は別途テストがあるので、インターフェイス回りのカバレッジを高めるのが狙い。
    /// </summary>
    [TestFixture]
    public class TestAsyncInterface
    {
        WindowsAppFriend app;

        /// <summary>
        /// 事前処理
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
        /// 通常テスト
        /// </summary>
        [Test]
        public void TestNormal()
        {
            //static呼び出し
            {
                Async async = new Async();
                app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic", async](100);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 100);
            }
            //変数メソッド呼び出し
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                Async async = new Async();
                v["SetPublicIntValueMethodPublic", async](100);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)v["PublicIntValue"]().Core, 100);
            }
        }

        /// <summary>
        /// 操作インターフェイス全網羅
        /// </summary>
        [Test]
        public void TestOperationAll()
        {           
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                Async async = new Async();
                v["SetPrivateIntValueMethodPublic", async](2);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 2);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 0);
            }
            //publicメンバで隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                Async async = new Async();
                v["SetPrivateIntValueMethodPublic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName), async](3);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 0);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 3);
            }

            //publicメンバ
            {
                Async async = new Async();
                app["FriendlyBaseTargetNet20.OperationTestClassPublic.SetPrivateIntValueStaticMethodPublic", async](2);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 0);
            }
            //publicメンバで隠された親メンバ
            {
                Async async = new Async();
                app["FriendlyBaseTargetNet20.OperationTestClassPublic.SetPrivateIntValueStaticMethodPublic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName), async](3);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }
            //publicメンバ
            {
                Async async = new Async();
                app[typeof(OperationTestClassPublic), "SetPrivateIntValueStaticMethodPublic", async](4);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                } 
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 4);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }
            //publicメンバで隠された親メンバ
            {
                Async async = new Async();
                app[typeof(OperationTestClassBasePublic), "SetPrivateIntValueStaticMethodPublic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName), async](5);
                while (!async.IsCompleted)
                {
                    Thread.Sleep(10);
                } 
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 4);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 5);
            }
        }

        /// <summary>
        /// 実行前に同期的に例外が発生するケース
        /// </summary>
        [Test]
        public void TestPreException()
        {
            try
            {
                Async async = new Async();
                app["xxx.yyy", async]();
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[xxx]" + Environment.NewLine +
                        "指定の型が見つかりません。" + Environment.NewLine +
                        "型フルネームが間違っているか、指定の型を含むモジュールが、まだロードされていない可能性があります。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// 実行中に例外発生
        /// </summary>
        [Test]
        public void TestExecutingException()
        {
            Async async = new Async();
            app[typeof(OperationTestClassPublic), "ThrowException", async]();
            while (!async.IsCompleted)
            {
                Thread.Sleep(10);
            }
            Assert.IsTrue(async.ExecutingException.Message.IndexOf("test") != -1);
        }

        /// <summary>
        /// 実行中に例外発生しなかった場合にExecutingExceptionにアクセスするとnullが返る
        /// </summary>
        [Test]
        public void TestExecutingExceptionNonThrow()
        {
            Async async = new Async();
            app[typeof(OperationTestClassPublic), "PublicIntValueStatic", async](1);
            async.WaitForCompletion();
            Assert.IsNull(async.ExecutingException);
        }

        /// <summary>
        /// 実行中にまだ処理が完了していない場合に、ExecutingExceptionにアクセスするとnullが返る
        /// </summary>
        [Test]
        public void TestExecutingExceptionNotComplate()
        {
            WindowControl window = WindowControl.FromZTop(app);
            Async async = new Async();
            app[typeof(MessageBox), "Show", async]("");
            Assert.IsNull(async.ExecutingException);
            WindowControl next = window.WaitForNextModal();

            next.IdentifyFromZIndex(1).SetFocus();
            next.IdentifyFromZIndex(1).SequentialMessage(
                new MessageInfo(0x0201, 0x0001, 0),
                new MessageInfo(0x0202, 0x0000, 0));

            async.WaitForCompletion();
        }

        /// <summary>
        /// 二回呼び出ししたために例外発生
        /// </summary>
        [Test]
        public void TestDoubleCallException()
        {
            try
            {
                Async async = new Async();
                app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic", async](3);
                app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic", async](3);
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "すでに実行されています。Asyncクラスの実行は一度だけです。複数回呼び出す場合は、再度Asyncクラスを生成してください。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// 不正に.がないstaticコールをした
        /// </summary>
        [Test]
        public void TestInvalidStaticCallException()
        {
            try
            {
                Async async = new Async();
                app["xxx", async]();
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "不正なstatic呼び出しです。操作情報には型が必要です。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// 処理が終わってから正常に変数の中に値が格納されること
        /// </summary>
        [Test]
        public void TestAsyncRetAndArgs()
        {
            AppVar arg = app.Dim();
            WindowControl window = WindowControl.FromZTop(app);
            Async async = new Async();
            AppVar ret = window.AppVar["AsyncValueTest", async](arg);
            WindowControl next = window.WaitForNextModal();
            next.IdentifyFromZIndex(1).SetFocus();
            next.IdentifyFromZIndex(1).SequentialMessage(
                new MessageInfo(0x0201, 0x0001, 0),
                new MessageInfo(0x0202, 0x0000, 0));
            while (!async.IsCompleted)
            {
                Thread.Sleep(10);
            }
            Assert.AreEqual((int)arg.Core, 1);
            Assert.AreEqual((int)ret.Core, 2);
        }

        /// <summary>
        /// WaitForCompletionのテスト
        /// </summary>
        [Test]
        public void TestWaitForCompletion()
        {
            AppVar arg = app.Dim();
            WindowControl window = WindowControl.FromZTop(app);
            Async async = new Async();
            AppVar ret = window.AppVar["AsyncValueTest", async](arg);
            WindowControl next = window.WaitForNextModal();
            WindowsAppExpander.LoadAssemblyFromFile(app, GetType().Assembly.Location);
            app[GetType(), "ClickByTimer"](next.IdentifyFromZIndex(1).Handle);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            async.WaitForCompletion();
            watch.Stop();
            Assert.IsTrue(1000 < watch.ElapsedMilliseconds);//１秒以上待っていること
            Assert.AreEqual((int)arg.Core, 1);
            Assert.AreEqual((int)ret.Core, 2);
        }

        const int BM_CLICK = 0x00F5;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// タイマーでクリック
        /// </summary>
        /// <param name="handle">ハンドル</param>
        public static void ClickByTimer(IntPtr handle)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Tick += delegate 
            {
                timer.Stop();
                SendMessage(handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            };
            timer.Interval = 2000;
            timer.Start();
        }

        /// <summary>
        /// 対象スレッドがフリーズしていても、非同期実行開始できること。
        /// </summary>
        [Test]
        public void TestAsyncTrigger()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            Async a1 = new Async();
            app[typeof(Thread), "Sleep", a1](1000);
            Async a2 = new Async();
            app[typeof(Thread), "Sleep", a2](1);

            //a1実行中でもa2の非同期実行はトリガがかかって抜けてくるので遅くとも100ミリ以内には終了する
            Assert.IsTrue(w.ElapsedMilliseconds < 100);
        }

        /// <summary>
        /// 完了設定
        /// </summary>
        [Test]
        public void TestSetCompleted()
        {
            Async a = new Async();
            a.SetCompleted();
            Assert.IsTrue(a.IsCompleted);
            a.WaitForCompletion();
        }

        /// <summary>
        /// 完了設定 例外処理
        /// </summary>
        [Test]
        public void TestSetCompletedException1()
        {
            Async a = new Async();
            app[typeof(Control), "FromHandle", a](IntPtr.Zero);
            try
            {
                a.SetCompleted();
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual("不正な終了指定です。通常このメソッドは使用しません。", e.Message);
            }
        }
        /// <summary>
        /// 完了設定 例外処理
        /// </summary>
        [Test]
        public void TestSetCompletedException2()
        {
            Async a = new Async();
            a.SetCompleted();
            try
            {
                app[typeof(Control), "FromHandle", a](IntPtr.Zero);
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual("すでに実行されています。Asyncクラスの実行は一度だけです。複数回呼び出す場合は、再度Asyncクラスを生成してください。", e.Message);
            }
        }

        /// <summary>
        /// テスト終了処理
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