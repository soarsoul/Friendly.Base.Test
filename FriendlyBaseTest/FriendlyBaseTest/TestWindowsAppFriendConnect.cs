using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using Codeer.Friendly.Windows;
using Codeer.Friendly;
using System.Threading;
using Codeer.Friendly.Windows.Grasp;
using System.Windows.Forms;

namespace FriendlyBaseTest
{
    /// <summary>
    /// WindowAppFriendでの接続テスト
    /// </summary>
    [TestFixture]
    class TestWindowsAppFriendConnect
    {
        /// <summary>
        /// CPU対象が異なる場合のテスト
        /// </summary>
        [Test]
        public void TestCpuTarget()
        {
            Process process = null;
            try
            {
                if (IntPtr.Size != 4)
                {
                    process = Process.Start(TargetPath.Path32);
                }
                else
                {
                    process = Process.Start(TargetPath.Path64);
                }
                WindowsAppFriend appCpuTest = new WindowsAppFriend(process, "2.0");
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual(e.Message, "プラットフォームターゲットがテスト対象とテストプロセスで異なります。合わせてください。");
            }
            finally
            {
                process.Kill();
            }
        }

        /// <summary>
        /// CLRの指定が不正な場合のテスト
        /// </summary>
        [Test]
        public void TestErrorCLR()
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
                WindowsAppFriend appCpuTest = new WindowsAppFriend(process, "3.5");
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual(e.Message, "対象プロセスの操作に失敗しました。" + Environment.NewLine +
                    "以下の可能性が考えられます。" + Environment.NewLine +
                    "・CLRのバージョン指定が間違っている。" + Environment.NewLine +
                    "・対象プロセスを操作する権限が足りていない。" + Environment.NewLine +
                    "・接続中に対象プロセスが終了した。" + Environment.NewLine +
                    "・指定のウィンドウハンドルのウィンドウが破棄された。" + Environment.NewLine +
                    "スプラッシュウィンドウを表示するアプリケーションの場合は、起動直後にメインウィンドウがスプラッシュウィンドウになっている場合があります。" + Environment.NewLine +
                    "明示的に期待のウィンドウのハンドルを指定してください。");
            }
            finally
            {
                process.Kill();
            }
        }

        /// <summary>
        /// マルチスレッドアクセスのテスト
        /// </summary>
        [Test]
        public void TestMultiThreadAccess()
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
                WindowsAppFriend app = null;

                Thread t = new Thread((ThreadStart)delegate { app = new WindowsAppFriend(process, "2.0"); });
                t.Start();
                t.Join();

                WindowControl top = WindowControl.FromZTop(app);
                Thread t2 = new Thread((ThreadStart)delegate
                {
                    WindowControl msg = top.WaitForNextModal();
                    WindowControl c = msg.IdentifyFromWindowText("OK");
                    c.SetFocus();
                    c.SequentialMessage(
                        new MessageInfo(0x0201, 0x0001, 0),
                        new MessageInfo(0x0202, 0x0000, 0));
                });
                t2.Start();
                app[typeof(MessageBox), "Show"]("");
                t2.Join();
            }
            finally
            {
                process.Kill();
            }
        }

        /// <summary>
        /// WindowsAppFriendから取得したAppVarがいる限り、WindowsAppFriendがGCで消されないテスト
        /// </summary>
        [Test]
        public void TestKeepAliveConnect()
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

                AppVar list = DimAppVar(process);
                GC.Collect();
                Thread.Sleep(2);
                for (int i = 0; i < 1000; i++)
                {
                    AddInLocal(list);
                    GC.Collect();
                    Thread.Sleep(2);
                }
            }
            finally
            {
                process.Kill();
            }
        }

        /// <summary>
        /// スプラッシュ対応
        /// </summary>
        [Test]
        public void TestSplash()
        {
            Process process = null;
            try
            {
                if (IntPtr.Size == 4)
                {
                    process = Process.Start(TargetPath.Path32, "splash");
                }
                else
                {
                    process = Process.Start(TargetPath.Path64, "splash");
                }

                try
                {
                    WindowsAppFriend w = new WindowsAppFriend(process, "2.0");
                }
                catch (FriendlyOperationException e)
                {
                    var expected1 = "対象プロセスの操作に失敗しました。" + Environment.NewLine +
                        "以下の可能性が考えられます。" + Environment.NewLine +
                        "・CLRのバージョン指定が間違っている。" + Environment.NewLine +
                        "・対象プロセスを操作する権限が足りていない。" + Environment.NewLine +
                        "・接続中に対象プロセスが終了した。" + Environment.NewLine +
                        "・指定のウィンドウハンドルのウィンドウが破棄された。" + Environment.NewLine +
                        "スプラッシュウィンドウを表示するアプリケーションの場合は、起動直後にメインウィンドウがスプラッシュウィンドウになっている場合があります。" + Environment.NewLine +
                        "明示的に期待のウィンドウのハンドルを指定してください。";

                    var expected2 = @"アプリケーションとの通信に失敗しました。" + Environment.NewLine +
                        "指定の実行対象スレッドに含まれるウィンドウは存在しません。" + Environment.NewLine +
                        "もしくは既に破棄されました。" + Environment.NewLine +
                        "スプラッシュウィンドウを表示するアプリケーションの場合は、起動直後にメインウィンドウがスプラッシュウィンドウになっている場合があります。" + Environment.NewLine +
                        "明示的に期待のウィンドウのハンドルを指定してください。";
                    Assert.IsTrue(e.Message == expected1 || e.Message == expected2);
                    return;
                }
                Assert.Fail();//タイミングによっては失敗しそうだが。まあ手動でやり直す。
            }
            finally
            {
                process.Kill();
            }
        }

        [Test]
        public void TestMultiDomain()
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
                var app = new WindowsAppFriend(process);
                WindowControl.FromZTop(app).AppVar["StartNewDomain"]();
                var newApp = app.AttachOtherDomains()[0];
                string name = (string)newApp[typeof(AppDomain), "CurrentDomain"]()["FriendlyName"]().Core;
                Assert.AreEqual("new domain", name);
                Assert.AreEqual(app.ProcessId, newApp.ProcessId);
            }
            finally
            {
                process.Kill();
            }
        }

        /// <summary>
        /// AppVarを宣言
        /// 宣言する元はこの関数内で接続する
        /// これでも、WindowsAppFriendがGCで消されないことを証明したい
        /// </summary>
        /// <param name="process">プロセス</param>
        /// <returns>AppVar</returns>
        private static AppVar DimAppVar(Process process)
        {
            WindowsAppFriend appCpuTest = new WindowsAppFriend(process, "2.0");
            AppVar list = appCpuTest.Dim(new List<int>());
            return list;
        }

        /// <summary>
        /// 念のためスタックを掘り下げてAddする
        /// </summary>
        /// <param name="list">リスト</param>
        private void AddInLocal(AppVar list)
        {
            list["Add"](0);
        }
    }
}
