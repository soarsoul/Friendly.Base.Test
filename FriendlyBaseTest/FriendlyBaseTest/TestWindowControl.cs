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
    /// TestWindowControlのインターフェイスに対する検査
    /// </summary>
    [TestFixture]
    public class TestWindowControl
    {
        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUp()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path32), "2.0");
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.Path64), "2.0");
            }
        }

        /// <summary>
        /// 用意
        /// </summary>
        public WindowsAppFriend SetUpWpf()
        {
            if (IntPtr.Size == 4)
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf32), "4.0");
            }
            else
            {
                return new WindowsAppFriend(Process.Start(TargetPath.PathWpf64), "4.0");
            }
        }

        /// <summary>
        /// AppVarが取得できないテスト
        /// </summary>
        [Test]
        public void TestNotGetAppVar()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            int id = 0;
            try
            {
                using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
                {
                    id = app.ProcessId;
                    WindowControl top = WindowControl.FromZTop(app);
                    Assert.AreEqual(top.AppVar, null);
                }
            }
            finally
            {
                Process.GetProcessById(id).CloseMainWindow();
            }
        }

        /// <summary>
        /// WindowControlのHandleとAppVarプロパティーのテスト
        /// </summary>
        [Test]
        public void TestHandleAndAppVar()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //AppVarのテスト
                targetForm.AppVar["Text"]("xxx");
                Assert.AreEqual(targetForm.AppVar["Text"]().ToString(), "xxx");

                //ハンドルのテスト
                WindowControl targetForm2 = new WindowControl(app, targetForm.Handle);
                targetForm2["Text"]("yyy");
                Assert.AreEqual(targetForm2["Text"]().ToString(), "yyy");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// WindowControlのHandleとAppVarプロパティーのテスト
        /// </summary>
        [Test]
        public void TestFriendlyOperation()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //呼び出し
                targetForm["Text"]("xxx");
                Assert.AreEqual(targetForm.AppVar["Text"]().ToString(), "xxx");

                //非同期呼び出し
                Async a = new Async();
                targetForm["Text", a]("yyy");
                while (!a.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual(targetForm.AppVar["Text"]().ToString(), "yyy");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 操作インターフェイスオーバーロード解決テスト
        /// </summary>
        [Test]
        public void TestFriendlyOperationResolveOverLoad()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                targetForm["Func", new OperationTypeInfo(typeof(TargetForm).FullName, typeof(Control).FullName)](null);
                Assert.AreEqual((int)targetForm["_overLoadCheckValue"]().Core, 1);
                targetForm["Func", new OperationTypeInfo(typeof(TargetForm).FullName, typeof(Form).FullName)](null);
                Assert.AreEqual((int)targetForm["_overLoadCheckValue"]().Core, 2);

                //非同期もテスト
                {
                    Async a = new Async();
                    targetForm["Func", new OperationTypeInfo(typeof(TargetForm).FullName, typeof(Control).FullName), a](null);
                    while (!a.IsCompleted)
                    {
                        Thread.Sleep(10);
                    }
                    Assert.AreEqual((int)targetForm["_overLoadCheckValue"]().Core, 1);
                }
                {
                    Async a = new Async();
                    targetForm["Func", new OperationTypeInfo(typeof(TargetForm).FullName, typeof(Form).FullName), a](null);
                    while (!a.IsCompleted)
                    {
                        Thread.Sleep(10);
                    }
                    Assert.AreEqual((int)targetForm["_overLoadCheckValue"]().Core, 2);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 操作インターフェイス使用不可能例外
        /// </summary>
        [Test]
        public void TestInvalidOperationWindow()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                try
                {
                    targetForm["xxx"]();
                    Assert.IsTrue(false);
                }
                catch(NotSupportedException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウはAppVarによるアクセスが不可能です。") != -1 &&
                                    e.Message.IndexOf("WindowText [MfcTestTarget]") != -1);
                }
                try
                {
                    targetForm["xxx", new OperationTypeInfo(typeof(TargetForm).FullName)]();
                    Assert.IsTrue(false);
                }
                catch (NotSupportedException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウはAppVarによるアクセスが不可能です。") != -1 &&
                                    e.Message.IndexOf("WindowText [MfcTestTarget]") != -1);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// リフレッシュ
        /// </summary>
        [Test]
        public void TestRefresh()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl dataGridView1 = targetForm.IdentifyFromZIndex(7);
                Click(dataGridView1, 64, 31);
                Click(dataGridView1, 64, 31);

                //リフレッシュすることにより、新たに表示されたテキストボックスにアクセス可能となる
                targetForm.Refresh();
                WindowControl editingControl = targetForm.IdentifyFromZIndex(7, 0, 0);
                editingControl["Text"]("xxx");
                editingControl.SequentialMessage(new MessageInfo(0x100, 0xD, 0x1C0001));
                AppVar cell = dataGridView1["[,]"](0, 0);
                Assert.AreEqual(cell["Value"]().ToString(), "xxx");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// オートリフレッシュを切った場合の動作
        /// </summary>
        [Test]
        public void TestNotAutoRefresh()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl dataGridView1 = targetForm.IdentifyFromZIndex(7);
                Click(dataGridView1, 64, 31);
                Click(dataGridView1, 64, 31);

                targetForm.AutoRefresh = false;
                try
                {
                    targetForm.IdentifyFromZIndex(7, 0, 0);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }

                //リフレッシュすることにより、新たに表示されたテキストボックスにアクセス可能となる
                targetForm.Refresh();
                WindowControl editingControl = targetForm.IdentifyFromZIndex(7, 0, 0);
                editingControl["Text"]("xxx");
                editingControl.SequentialMessage(new MessageInfo(0x100, 0xD, 0x1C0001));
                AppVar cell = dataGridView1["[,]"](0, 0);
                Assert.AreEqual(cell["Value"]().ToString(), "xxx");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// オートリフレッシュ
        /// </summary>
        [Test]
        public void TestAutoRefresh()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl dataGridView1 = targetForm.IdentifyFromZIndex(7);
                Click(dataGridView1, 64, 31);
                Click(dataGridView1, 64, 31);

                WindowControl editingControl = targetForm.IdentifyFromZIndex(7, 0, 0);
                editingControl["Text"]("xxx");
                editingControl.SequentialMessage(new MessageInfo(0x100, 0xD, 0x1C0001));
                AppVar cell = dataGridView1["[,]"](0, 0);
                Assert.AreEqual(cell["Value"]().ToString(), "xxx");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        //@@@AutoRefreshのテスト

        /// <summary>
        /// トップウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestTopWindow()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                //ZTopから取得
                {
                    WindowControl targetForm = WindowControl.FromZTop(app);
                    targetForm["Text"]("xxx");
                    Assert.AreEqual(targetForm["Text"]().ToString(), "xxx");
                }
                //ウィンドウハンドルから取得
                AppVar v = null;
                {
                    WindowControl targetForm = new WindowControl(app, process.MainWindowHandle);
                    targetForm["Text"]("aaa");
                    Assert.AreEqual(targetForm["Text"]().ToString(), "aaa");
                    v = targetForm.AppVar;
                }
                //AppVarから取得
                {
                    WindowControl targetForm = new WindowControl(v);
                    targetForm["Text"]("bbb");
                    Assert.AreEqual(targetForm["Text"]().ToString(), "bbb");
                }
                //AppVarから取得(非推奨インターフェイス)
                {
                    WindowControl targetForm = new WindowControl(app, v);
                    targetForm["Text"]("ccc");
                    Assert.AreEqual(targetForm["Text"]().ToString(), "ccc");
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// WPFのウィンドウもウィンドウハンドルから取得できることを確認
        /// </summary>
        [Test]
        public void TestGetWindowFromHandle()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUpWpf())
            {
                process = Process.GetProcessById(app.ProcessId);
                //ウィンドウハンドルから取得
                AppVar v = null;
                {
                    WindowControl targetForm = new WindowControl(app, process.MainWindowHandle);
                    targetForm["Title"]("aaa");
                    Assert.AreEqual(targetForm["Title"]().ToString(), "aaa");
                    v = targetForm.AppVar;
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }
        
        /// <summary>
        /// 子ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestGetChild()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                var list = new List<string>();
                foreach (var e in targetForm.GetChildren())
                {
                    list.Add(e.TypeFullName + ":" + e.GetWindowText());
                }
                var ret = new[] {
                    "System.Windows.Forms.Button:別スレッドWindow起動",
                    "FriendlyBaseTargetNet20.TestUserControl:",
                    "System.Windows.Forms.GroupBox:タイプとウィンドウクラスが同一",
                    "System.Windows.Forms.GroupBox:WindowTextが同じ",
                    "System.Windows.Forms.PictureBox:",
                    "System.Windows.Forms.Panel:",
                    "System.Windows.Forms.Label:↓ここに矩形の重なったオブジェクトがある",
                    "System.Windows.Forms.DataGridView:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "System.Windows.Forms.Button:button1",
                    "System.Windows.Forms.Panel:",
                };
                Assert.AreEqual(ret.Length, list.Count);
                for (int i = 0; i < ret.Length; i++)
                {
                    Assert.AreEqual(ret[i], list[i]);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子孫ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestGetDescendants()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                var list = new List<string>();
                foreach (var e in targetForm.GetDescendants())
                {
                    list.Add(e.TypeFullName + ":" + e.GetWindowText());
                }
                var ret = new[] {
                    "System.Windows.Forms.Button:別スレッドWindow起動",
                    "FriendlyBaseTargetNet20.TestUserControl:",
                    "System.Windows.Forms.TextBox:xxx",
                    "System.Windows.Forms.GroupBox:タイプとウィンドウクラスが同一",
                    "System.Windows.Forms.CheckBox:checkBox2",
                    "System.Windows.Forms.CheckBox:checkBox1",
                    "System.Windows.Forms.GroupBox:WindowTextが同じ",
                    "System.Windows.Forms.Label:yyy",
                    "System.Windows.Forms.Label:yyy",
                    "System.Windows.Forms.PictureBox:",
                    "System.Windows.Forms.Panel:",
                    "System.Windows.Forms.Label:↓ここに矩形の重なったオブジェクトがある",
                    "System.Windows.Forms.DataGridView:",
                    "System.Windows.Forms.HScrollBar:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "FriendlyBaseTargetNet20.PanelEx:",
                    "System.Windows.Forms.Button:button1",
                    "System.Windows.Forms.Panel:",
                    "System.Windows.Forms.Panel:",
                    "System.Windows.Forms.SplitContainer:",
                    "System.Windows.Forms.SplitterPanel:",
                    "System.Windows.Forms.CheckBox:checkBox3",
                    "System.Windows.Forms.SplitterPanel:",
                    "System.Windows.Forms.CheckBox:checkBox3"
                };
                Assert.AreEqual(ret.Length, list.Count);
                for (int i = 0; i < ret.Length; i++)
                {
                    Assert.AreEqual(ret[i], list[i]);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }
        /// <summary>
        /// 子ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestIdentifyChildWindow()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //.Netフィールドパスから取得
                {
                    AppVar _textBox = targetForm["_testUserControl"]()["_textBox"]();
                    _textBox["Text"]("a");
                    Assert.AreEqual(_textBox["Text"]().ToString(), "a");
                }
                //ZIndexから取得
                {
                    WindowControl _textBox = targetForm.IdentifyFromZIndex(1, 0);
                    _textBox["Text"]("b");
                    Assert.AreEqual(_textBox["Text"]().ToString(), "b");
                    _textBox["Text"]("xxx");
                }
                //テキストから取得
                {
                    WindowControl _textBox = targetForm.IdentifyFromWindowText("xxx");
                    _textBox["Text"]("c");
                    Assert.AreEqual(_textBox["Text"]().ToString(), "c");
                }
                //矩形から取得
                {
                    WindowControl _textBox = targetForm.IdentifyFromBounds(49, 161, 100, 19);
                    _textBox["Text"]("d");
                    Assert.AreEqual(_textBox["Text"]().ToString(), "d");
                }
                //.Netタイプから取得
                {
                    AppVar _textBox = targetForm.IdentifyFromTypeFullName("System.Windows.Forms.TextBox");
                    _textBox["Text"]("e");
                    Assert.AreEqual(_textBox["Text"]().ToString(), "e");
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウの取得処理 例外処理
        /// </summary>
        [Test]
        public void TestIdentifyChildWindowException()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //.Netフィールドパスから取得
                try
                {
                    targetForm["_testUserControlX"]();
                    Assert.IsTrue(false);
                }
                catch (FriendlyOperationException e)
                {
                    string message = "[型 : TargetForm][操作 : _testUserControlX()]" + Environment.NewLine +
                                "指定の操作が見つかりませんでした。";
                    Assert.AreEqual(e.Message, message);
                }

                //ZIndexから取得
                try
                {
                    targetForm.IdentifyFromZIndex(1, 0, 5);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }

                //テキストから取得
                try
                {
                    targetForm.IdentifyFromWindowText("qqq");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }
                try
                {
                    targetForm.IdentifyFromWindowText("yyy");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが複数発見され、特定できませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }

                //矩形から取得
                try
                {
                    targetForm.IdentifyFromBounds(-1, -1, 1, 1);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }
                try
                {
                    targetForm.IdentifyFromBounds(10, 282, 98, 50);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが複数発見され、特定できませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }

                //.Netタイプから取得                
                try
                {
                    targetForm.IdentifyFromTypeFullName("System.Windows.Forms.XXX");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }
                try
                {
                    targetForm.IdentifyFromTypeFullName("System.Windows.Forms.CheckBox");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが複数発見され、特定できませんでした。") != -1 &&
                        e.Message.IndexOf("WindowText [TargetForm]") != -1);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウ取得処理
        /// </summary>
        [Test]
        public void TestGetChildWindow()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //テキストから取得
                {
                    WindowControl[] control = targetForm.GetFromWindowText("yyy");
                    Assert.AreEqual(control.Length, 2);
                    for (int i = 0; i < control.Length; i++)
                    {
                        control[i]["Text"](i.ToString());
                        Assert.AreEqual(control[i]["Text"]().ToString(), i.ToString());
                    }
                }

                //矩形から取得
                {
                    WindowControl[] control = targetForm.GetFromBounds(10, 282, 98, 50);
                    Assert.AreEqual(control.Length, 2);
                    for (int i = 0; i < control.Length; i++)
                    {
                        control[i]["Text"](i.ToString());
                        Assert.AreEqual(control[i]["Text"]().ToString(), i.ToString());
                    }
                }

                //.Netタイプから取得      
                {
                    AppVar[] control = targetForm.GetFromTypeFullName("System.Windows.Forms.CheckBox");
                    Assert.AreEqual(control.Length, 4);
                    for (int i = 0; i < control.Length; i++)
                    {
                        control[i]["Text"](i.ToString());
                        Assert.AreEqual(control[i]["Text"]().ToString(), i.ToString());
                    }
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestWpfTopWindowFromHandle()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUpWpf())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = new WindowControl(app, process.MainWindowHandle);
                AppVar textBox1 = targetForm["_textBox"]();
                textBox1["Text"]("a");
                Assert.AreEqual(textBox1["Text"]().ToString(), "a");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestIdentifyChildWpf()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUpWpf())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                AppVar _button = targetForm["_button"]();
                AppVar _buttonLogical = targetForm.IdentifyFromLogicalTreeIndex(0, 0, 0, 0, 0, 1, 0);
                AppVar _buttonVisual = targetForm.IdentifyFromVisualTreeIndex(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_button, _buttonLogical).Core);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_button, _buttonVisual).Core);

                AppVar _checkBox = targetForm["_checkBox"]();
                AppVar _checkBoxLogical = targetForm.IdentifyFromLogicalTreeIndex(0, 0, 0, 0, 0, 1, 1, 0);
                AppVar _checkBoxVisual = targetForm.IdentifyFromVisualTreeIndex(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_checkBox, _checkBoxLogical).Core);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_checkBox, _checkBoxVisual).Core);

                AppVar _textBox = targetForm["_textBox"]();
                AppVar _textBoxLogical = targetForm.IdentifyFromLogicalTreeIndex(0, 1, 2);
                AppVar _textBoxVisual = targetForm.IdentifyFromVisualTreeIndex(0, 0, 0, 0, 1, 2);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_textBox, _textBoxLogical).Core);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_textBox, _textBoxVisual).Core);

                AppVar _slider = targetForm["_slider"]();
                AppVar _sliderLogical = targetForm.IdentifyFromLogicalTreeIndex(0, 1, 1);
                AppVar _sliderVisual = targetForm.IdentifyFromVisualTreeIndex(0, 0, 0, 0, 1, 1);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_slider, _sliderLogical).Core);
                Assert.IsTrue((bool)app[typeof(object), "ReferenceEquals"](_slider, _sliderVisual).Core);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウの取得処理
        /// </summary>
        [Test]
        public void TestIdentifyChildWpfException()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUpWpf())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //.Netフィールドパスから取得
                try
                {
                    targetForm["textBox2"]();
                    Assert.IsTrue(false);
                }
                catch (FriendlyOperationException e)
                {
                    string message = "[型 : MainWindow][操作 : textBox2()]" + Environment.NewLine +
                            "指定の操作が見つかりませんでした。";
                    Assert.AreEqual(e.Message, message);
                }

                //ロジカルツリーインデックスから取得
                try
                {
                    targetForm.IdentifyFromLogicalTreeIndex(0, 0, 0, 1, 0, 10);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                        e.Message.IndexOf("TypeFullName(.Net) [" + targetForm.TypeFullName + "]") != -1);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウ取得処理(ネイティブ)
        /// </summary>
        [Test]
        public void TestIdentifyChildNative()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                //ダイアログIDで取得
                {
                    WindowControl edit = targetForm.IdentifyFromDialogId(1000);
                    edit.SetWindowText("xxx");
                    Assert.AreEqual(edit.GetWindowText(), "xxx");
                }
                {
                    WindowControl edit = targetForm.IdentifyFromDialogId(1004, 1004, 1004);
                    edit.SetWindowText("xxx");
                    Assert.AreEqual(edit.GetWindowText(), "xxx");
                }
                //ウィンドウクラスで取得
                {
                    WindowControl edit = targetForm.IdentifyFromWindowClass("Static");
                    edit.SetWindowText("yyy");
                    Assert.AreEqual(edit.GetWindowText(), "yyy");
                }
                //1004

            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// ダイアログIDでの例外テスト
        /// </summary>
        [Test]
        public void TestDialogIdException()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                Async async;
                Click(targetForm.IdentifyFromDialogId(1), 0, 0, out async);
                WindowControl next = targetForm.WaitForNextModal();
                try
                {
                    next.IdentifyFromDialogId(0, 1001);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf(
                        "同一階層に指定のダイアログIDを持つウィンドウが複数存在するため、" + Environment.NewLine + 
                        "ウィンドウを特定することができませんでした。" + Environment.NewLine + 
                        "以下の情報を持つウィンドウに対して操作を実施しました。" + Environment.NewLine + 
                        "意図したウィンドウですか？") != -1 &&
                        e.Message.IndexOf("WindowText [xx5]") != -1);
                }
                Click(next.IdentifyFromDialogId(1), 0, 0);
                next.WaitForDestroy();
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// ウィンドウ情報取得メソッドテスト(ネイティブ)
        /// </summary>
        [Test]
        public void TestNativeWindowInfo()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl comboEx = targetForm.IdentifyFromDialogId(1004);
                Assert.AreEqual(1004, comboEx.DialogId);
                Assert.AreEqual("ComboBoxEx32", comboEx.WindowClassName);
                Assert.AreEqual(targetForm.Handle, comboEx.ParentWindow.Handle);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// ウィンドウ情報取得メソッドテスト(ネイティブ)
        /// </summary>
        [Test]
        public void TestNetWindowInfo()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                Assert.AreEqual(typeof(TargetForm).FullName, targetForm.TypeFullName);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// GetTopLevelWindowsテスト
        /// </summary>
        [Test]
        public void TestGetTopLevelWindow()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl[] all = WindowControl.GetTopLevelWindows(app);
                Assert.AreEqual(2, all.Length);
                Assert.AreEqual(targetForm.Handle, all[0].Handle);
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, all[1].Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromWindowTextテスト
        /// </summary>
        [Test]
        public void TestIdentifyFromWindowText()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.IdentifyFromWindowText(app, "abc");
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromWindowTextの例外テスト
        /// </summary>
        [Test]
        public void TestIdentifyFromWindowTextException()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                for (int i = 0; i < 2; i++)
                {
                    AppVar newForm = app.Dim(new NewInfo<Form>());
                    newForm["Text"]("abc");
                    newForm["Show"]();
                }
                try
                {
                    WindowControl.IdentifyFromWindowText(app, "xxx");
                    Assert.Fail();
                }
                catch (WindowIdentifyException e)
                {
                    Assert.AreEqual("指定のウィンドウが見つかりませんでした。", e.Message);
                }
                try
                {
                    WindowControl.IdentifyFromWindowText(app, "abc");
                    Assert.Fail();
                }
                catch (WindowIdentifyException e)
                {
                    Assert.AreEqual("指定のウィンドウが複数発見され、特定できませんでした。", e.Message);
                }
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// GetFromWindowTextテスト
        /// </summary>
        [Test]
        public void TestGetFromWindowText()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                for (int i = 0; i < 2; i++)
                {
                    AppVar newForm = app.Dim(new NewInfo<Form>());
                    newForm["Text"]("abc");
                    newForm["Show"]();
                }
                WindowControl[] windows = WindowControl.GetFromWindowText(app, "abc");
                Assert.AreEqual(2, windows.Length);
                Assert.AreEqual("abc", windows[0].GetWindowText());
                Assert.AreEqual("abc", windows[0].GetWindowText());
                Assert.IsFalse(windows[0].Handle == windows[1].Handle);
                Assert.AreEqual(0, WindowControl.GetFromWindowText(app, "xxx").Length);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromTypeFullNameテスト
        /// </summary>
        [Test]
        public void TestIdentifyFromTypeFullName()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.IdentifyFromTypeFullName(app, typeof(Form).FullName);
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromTypeFullNameの例外テスト
        /// </summary>
        [Test]
        public void TestIdentifyFromTypeFullNameException()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                for (int i = 0; i < 2; i++)
                {
                    AppVar newForm = app.Dim(new NewInfo<Form>());
                    newForm["Text"]("abc");
                    newForm["Show"]();
                }
                try
                {
                    WindowControl.IdentifyFromTypeFullName(app, "xxx");
                    Assert.Fail();
                }
                catch (WindowIdentifyException e)
                {
                    Assert.AreEqual("指定のウィンドウが見つかりませんでした。", e.Message);
                }
                try
                {
                    WindowControl.IdentifyFromTypeFullName(app, typeof(Form).FullName);
                    Assert.Fail();
                }
                catch (WindowIdentifyException e)
                {
                    Assert.AreEqual("指定のウィンドウが複数発見され、特定できませんでした。", e.Message);
                }
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// GetFromTypeFullNameテスト
        /// </summary>
        [Test]
        public void TestGetFromTypeFullName()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                for (int i = 0; i < 2; i++)
                {
                    AppVar newForm = app.Dim(new NewInfo<Form>());
                    newForm["Text"]("abc");
                    newForm["Show"]();
                }
                WindowControl[] windows = WindowControl.GetFromTypeFullName(app, typeof(Form).FullName);
                Assert.AreEqual(2, windows.Length);
                Assert.AreEqual("abc", windows[0].GetWindowText());
                Assert.AreEqual("abc", windows[0].GetWindowText());
                Assert.IsFalse(windows[0].Handle == windows[1].Handle);
                Assert.AreEqual(0, WindowControl.GetFromTypeFullName(app, "xxx").Length);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// 子ウィンドウ取得処理(ネイティブ)
        /// </summary>
        [Test]
        public void TestIdentifyChildNativeException()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                //ダイアログIDから特定しようとするが発見できず失敗
                try
                {
                    targetForm.IdentifyFromDialogId(5);
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 &&
                                    e.Message.IndexOf("WindowText [MfcTestTarget]") != -1);
                }
                //ウィンドウクラスから特定しようとするが発見できず失敗
                try
                {
                    targetForm.IdentifyFromWindowClass("TTT");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが見つかりませんでした。") != -1 && 
                                    e.Message.IndexOf("WindowText [MfcTestTarget]") != -1);
                }
                //ウィンドウクラスから特定しようとするが複数あり取得失敗
                try
                {
                    targetForm.IdentifyFromWindowClass("Button");
                    Assert.IsTrue(false);
                }
                catch (WindowIdentifyException e)
                {
                    Assert.IsTrue(e.Message.IndexOf("指定のウィンドウが複数発見され、特定できませんでした。") != -1 &&
                                    e.Message.IndexOf("WindowText [MfcTestTarget]") != -1);
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 子ウィンドウ取得処理(ネイティブ)
        /// </summary>
        [Test]
        public void TestGetChildNative()
        {
            if (IntPtr.Size != 4)
            {
                return;
            }
            Process process = null;
            using (WindowsAppFriend app = new WindowsAppFriend(Process.Start(TargetPath.PathMfc), "2.0"))
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                //ウィンドウクラスで取得
                {
                    WindowControl[] button = targetForm.GetFromWindowClass("Button");
                    Assert.AreEqual(button.Length, 4);
                    for (int i = 0; i < button.Length; i++)
                    {
                        button[i].SetWindowText(i.ToString());
                        Assert.AreEqual(button[i].GetWindowText(), i.ToString());
                    }
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// Windowテキストのテスト
        /// バッファサイズが絡むので特殊な文字列も送信する
        /// 現在1文字最大8バイトでとっている
        /// </summary>
        [Test]
        public void TestWindowText()
        {
            //念のため特殊な文字の送受信も試す
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                targetForm.SetWindowText("☠☠☠");
                Assert.AreEqual(targetForm.GetWindowText(), "☠☠☠");
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// SendMessage系のテスト
        /// </summary>
        [Test]
        public void TestSendMessage()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);

                //IntPtr
                {
                    targetForm.SendMessage(0x8001, LongToIntPtr(0xfffffffd), LongToIntPtr(0xfffffffc));
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffffd));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffffc));
                }
                //IntPtr
                {
                    Async async = new Async();
                    targetForm.SendMessage(0x8002, LongToIntPtr(0xfffffff1), LongToIntPtr(0xfffffff2), async);
                    Assert.IsFalse(async.IsCompleted);
                    WindowControl next = targetForm.WaitForNextModal();
                    WindowControl button1 = next.IdentifyFromZIndex(1);
                    Click(button1, 1, 1);
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffff1));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffff2));
                }

                //long
                {
                    targetForm.SequentialMessage(new MessageInfo(0x8001, 0xfffffffb, 0xfffffffa));
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffffb));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffffa));
                }

                //long
                {
                    Async async = new Async();
                    targetForm.SequentialMessage(async, new MessageInfo(0x8002, 0xfffffff9, 0xfffffff8));
                    Assert.IsFalse(async.IsCompleted);
                    WindowControl next = targetForm.WaitForNextModal();
                    WindowControl button1 = next.IdentifyFromZIndex(1);
                    Click(button1, 1, 1);
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffff9));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffff8));
                    while (!async.IsCompleted)
                    {
                        Thread.Sleep(10);
                    }
                }
                //IntPtr
                {
                    targetForm.SequentialMessage(new MessageInfo(0x8001, LongToIntPtr(0xfffffffb), LongToIntPtr(0xfffffffa)));
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffffb));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffffa));
                }
                //IntPtr
                {
                    Async async = new Async();
                    targetForm.SequentialMessage(async, new MessageInfo(0x8002, LongToIntPtr(0xfffffff9), LongToIntPtr(0xfffffff8)));
                    Assert.IsFalse(async.IsCompleted);
                    WindowControl next = targetForm.WaitForNextModal();
                    WindowControl button1 = next.IdentifyFromZIndex(1);
                    Click(button1, 1, 1);
                    Assert.AreEqual((IntPtr)targetForm["_wparam"]().Core, LongToIntPtr(0xfffffff9));
                    Assert.AreEqual((IntPtr)targetForm["_lparam"]().Core, LongToIntPtr(0xfffffff8));
                    while (!async.IsCompleted)
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// フォーカスの設定
        /// </summary>
        [Test]
        public void TestSetFocus()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl checkBox1 = new WindowControl(targetForm["checkBox1"]());
                checkBox1.SetFocus();
                Assert.IsTrue((bool)checkBox1["Focused"]().Core);
                WindowControl checkBox2 = new WindowControl(targetForm["checkBox2"]());
                checkBox2.SetFocus();
                Assert.IsTrue((bool)checkBox2["Focused"]().Core);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 次画面待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNext()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);

                //Z待ち
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl clickAsyncPanel = new WindowControl(targetForm["clickAsyncPanel"]());
                {
                    Async async;
                    Click(clickAsyncPanel, 1, 2, out async);
                    WindowControl next = targetForm.WaitForNextZTop();//非推奨でも一応テスト
                    Click(next.IdentifyFromZIndex(1), 1, 1);
                }
                {
                    Async async;
                    Click(clickAsyncPanel, 1, 2, out async);
                    WindowControl next = targetForm.WaitForNextZTop(async);//非推奨でも一応テスト
                    Click(next.IdentifyFromZIndex(1), 1, 1);
                }
                //画面は表示されないが、asyncが完了するので固まらず、抜けてくる
                {
                    Async async = new Async();
                    app[typeof(Control), "FromHandle", async](targetForm.Handle);
                    WindowControl next = targetForm.WaitForNextZTop(async);//非推奨でも一応テスト
                    Assert.AreEqual(next, null);
                }
            }

            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 次モーダル待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNextModal()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);

                //Z待ち
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl clickAsyncPanel = new WindowControl(targetForm["clickAsyncPanel"]());
                {
                    Async async;
                    Click(clickAsyncPanel, 1, 2, out async);
                    WindowControl next = targetForm.WaitForNextModal();
                    Click(next.IdentifyFromZIndex(1), 1, 1);
                }
                {
                    Async async;
                    Click(clickAsyncPanel, 1, 2, out async);
                    WindowControl next = targetForm.WaitForNextModal(async);
                    Click(next.IdentifyFromZIndex(1), 1, 1);
                }
                //画面は表示されないが、asyncが完了するので固まらず、抜けてくる
                {
                    Async async = new Async();
                    app[typeof(Control), "FromHandle", async](targetForm.Handle);
                    WindowControl next = targetForm.WaitForNextModal(async);
                    Assert.AreEqual(next, null);
                }
            }

            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 次モーダル待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNextModalDestroyed()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                var form = app.Dim(new NewInfo<Form>());
                var w = new WindowControl(form);
                form["Close"]();
                var next = w.WaitForNextModal();
                Assert.IsNull(next);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// 次モーダル待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNextModalDestroyedAsync()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                var form = app.Dim(new NewInfo<Form>());
                var w = new WindowControl(form);
                var a = new Async();
                form["Close", a]();
                var next = w.WaitForNextModal(a);
                Assert.IsNull(next);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        const int BM_CLICK = 0x00F5;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// WaitForDestroyとIsWindowのテスト
        /// </summary>
        [Test]
        public void TestWaitForDestroyAndIsWindow()
        {
            Process process = null;
            using (WindowsAppFriend app = SetUp())
            {
                process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                WindowControl clickAsyncPanel = new WindowControl(targetForm["clickAsyncPanel"]());

                Async async;
                Click(clickAsyncPanel, 1, 2, out async);
                WindowControl next = targetForm.WaitForNextModal();
                IntPtr handleButton = next.IdentifyFromZIndex(1).Handle;

                Thread thread = new Thread((ThreadStart)delegate
                    {
                        Thread.Sleep(3000);
                        SendMessage(handleButton, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    });
                thread.Start();

                Async async2 = new Async();
                targetForm["Text", async2]();

                //すぐに抜けること。でも余裕をもって100
                Stopwatch w = new Stopwatch();
                w.Start();
                next.WaitForDestroy(async2);
                w.Stop();
                Assert.IsTrue(w.ElapsedMilliseconds < 100);

                //終了スレッドがボタンを押してから抜けること
                w = new Stopwatch();
                w.Start();
                Assert.IsTrue(next.IsWindow());
                next.WaitForDestroy();
                Assert.IsFalse(next.IsWindow());
                w.Stop();
                Assert.IsTrue(2000 < w.ElapsedMilliseconds);
            }
            if (process != null)
            {
                process.CloseMainWindow();
            }
        }

        /// <summary>
        /// IdentifyFromWindowTextテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromWindowText()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.WaitForIdentifyFromWindowText(app, "abc");
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromWindowTextテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromWindowTextAsync()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.WaitForIdentifyFromWindowText(app, "abc", new Async());
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromWindowTextテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromWindowTextAsyncNull()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                Async async = new Async();
                app[typeof(Thread), "Sleep", async](1000);
                WindowControl w = WindowControl.WaitForIdentifyFromWindowText(app, "abc", async);
                Assert.IsNull(w);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromTypeFullNameテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromTypeFullName()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.WaitForIdentifyFromTypeFullName(app, typeof(Form).FullName);
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromTypeFullNameテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromTypeFullNameAsync()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                AppVar newForm = app.Dim(new NewInfo<Form>());
                newForm["Text"]("abc");
                newForm["Show"]();
                WindowControl w = WindowControl.WaitForIdentifyFromTypeFullName(app, typeof(Form).FullName, new Async());
                Assert.AreEqual((IntPtr)newForm["Handle"]().Core, w.Handle);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// IdentifyFromTypeFullNameテスト
        /// </summary>
        [Test]
        public void TestWaitForIdentifyFromTypeFullNameAsyncNull()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                Async async = new Async();
                app[typeof(Thread), "Sleep", async](1000);
                WindowControl w = WindowControl.WaitForIdentifyFromTypeFullName(app, typeof(Form).FullName, async);
                Assert.IsNull(w);
                targetForm["Close", new Async()]();
            }
        }

        /// <summary>
        /// 待ち中に対象が終了した場合のテスト
        /// </summary>
        [Test]
        public void TestWaitTargetEnd()
        {
            using (WindowsAppFriend app = SetUp())
            {
                Process process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                Thread thread = new Thread((ThreadStart)delegate
                {
                    Thread.Sleep(1000);
                    process.CloseMainWindow();
                });
                thread.Start();

                try
                {
                    targetForm.WaitForNextModal();
                }
                catch (FriendlyOperationException e)
                {
                    string message = "アプリケーションとの通信に失敗しました。" + Environment.NewLine +
                        "対象アプリケーションが通信不能な状態になったか、" + Environment.NewLine +
                        "シリアライズ不可能な型のデータを転送しようとした可能性があります。";
                    Assert.AreEqual(e.Message, message);
                }
            }

            using (WindowsAppFriend app = SetUp())
            {
                Process process = Process.GetProcessById(app.ProcessId);
                WindowControl targetForm = WindowControl.FromZTop(app);
                Thread thread = new Thread((ThreadStart)delegate
                {
                    Thread.Sleep(1000);
                    process.CloseMainWindow();
                });
                thread.Start();

                try
                {
                    targetForm.WaitForDestroy();
                }
                catch (FriendlyOperationException e)
                {
                    string message = "アプリケーションとの通信に失敗しました。" + Environment.NewLine +
                        "対象アプリケーションが通信不能な状態になったか、" + Environment.NewLine +
                        "シリアライズ不可能な型のデータを転送しようとした可能性があります。";
                    Assert.AreEqual(e.Message, message);
                }
            }

            using (WindowsAppFriend app = SetUp())
            {
                Process process = Process.GetProcessById(app.ProcessId);
                process.CloseMainWindow();
                
                try
                {
                    WindowControl targetForm = WindowControl.FromZTop(app);
                }
                catch (FriendlyOperationException e)
                {
                    string message = "アプリケーションとの通信に失敗しました。" + Environment.NewLine +
                        "対象アプリケーションが通信不能な状態になったか、" + Environment.NewLine +
                        "シリアライズ不可能な型のデータを転送しようとした可能性があります。";
                    Assert.AreEqual(e.Message, message);
                }
            }
        }

        /// <summary>
        /// 最初のアクセステスト
        /// 正しくDLLがロードされ、問題なく動作することの確認
        /// </summary>
        [Test]
        public void TestFirstAccess()
        {
            //AppVar
            using (WindowsAppFriend app = SetUp())
            {
                AppVar main = app[typeof(Control), "FromHandle"](Process.GetProcessById(app.ProcessId).MainWindowHandle);
                WindowControl w = new WindowControl(main);
                main["Close", new Async()]();
            }
            //Handle
            using (WindowsAppFriend app = SetUp())
            {
                AppVar main = app[typeof(Control), "FromHandle"](Process.GetProcessById(app.ProcessId).MainWindowHandle);
                WindowControl w = new WindowControl(main);
                main["Close", new Async()]();
            }
            //各種static
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.GetFromTypeFullName(app, "");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.GetFromWindowClass(app, "");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.GetFromWindowText(app, "");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.GetTopLevelWindows(app);
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.IdentifyFromTypeFullName(app, "FriendlyBaseTargetNet20.TargetForm");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.IdentifyFromWindowClass(app, "WindowsForms10.Window.8.app.0.378734a");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.IdentifyFromWindowText(app, "TargetForm");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromTypeFullName(app, "FriendlyBaseTargetNet20.TargetForm");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromTypeFullName(app, "FriendlyBaseTargetNet20.TargetForm", new Async());
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromWindowClass(app, "WindowsForms10.Window.8.app.0.378734a");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromWindowClass(app, "WindowsForms10.Window.8.app.0.378734a", new Async());
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromWindowText(app, "TargetForm");
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl.WaitForIdentifyFromWindowText(app, "TargetForm", new Async());
                WindowControl.FromZTop(app)["Close", new Async()]();
            }
        }

        /// <summary>
        /// Sizeテスト
        /// </summary>
        [Test]
        public void TestSize()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                Assert.AreEqual((Size)targetForm["Size"]().Core, targetForm.Size);
                targetForm.Close(new Async());
            }
        }

        /// <summary>
        /// PointToScreenテスト
        /// </summary>
        [Test]
        public void TestPointToScreen()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                Assert.AreEqual((Point)targetForm["PointToScreen"](new Point()).Core, targetForm.PointToScreen(new Point()));
                targetForm.Close(new Async());
            }
        }

        [Test]
        public void TestClose()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                var form = app.Dim(new NewInfo<Form>());
                form["Show"]();
                var ctrl = new WindowControl(form);
                ctrl.Close();
                ctrl.WaitForDestroy();
                targetForm.Close(new Async());
            }
        }

        [Test]
        public void TestActivate()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                var form = app.Dim(new NewInfo<Form>());
                form["Show"]();

                var active = app[typeof(Form), "ActiveForm"]();
                Assert.AreEqual((IntPtr)active["Handle"]().Core, (IntPtr)form["Handle"]().Core);

                targetForm.Activate();
                active = app[typeof(Form), "ActiveForm"]();
                Assert.AreEqual((IntPtr)active["Handle"]().Core, (IntPtr)targetForm["Handle"]().Core);

                targetForm.Close(new Async());
            }
        }

        /// <summary>
        /// 次ウィンドウ待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNextWindow()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                var next = WindowControl.WaitForNextWindow(app, () =>
                {
                    var form = app.Dim(new NewInfo<Form>());
                    form["Show"]();
                });
                next.Close();
                targetForm.Close(new Async());
            }
        }

        /// <summary>
        /// 次ウィンドウ取得テスト
        /// </summary>
        [Test]
        public void TestGetNextWindow()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                var nexts = WindowControl.GetNextWindows(app, () =>
                {
                    app.Dim(new NewInfo<Form>())["Show"]();
                    app.Dim(new NewInfo<Form>())["Show"]();
                });
                Assert.AreEqual(2, nexts.Length);
                targetForm.Close(new Async());
            }
        }

        /// <summary>
        /// 次ウィンドウ待ちテスト
        /// </summary>
        [Test]
        public void TestWaitNextWindowAsync()
        {
            using (WindowsAppFriend app = SetUp())
            {
                WindowControl targetForm = WindowControl.FromZTop(app);
                var async = new Async();
                var next = WindowControl.WaitForNextWindow(app, () =>
                {
                    var form = app.Dim(new NewInfo<Form>());
                    form["Show", async]();
                    form["Close"]();
                },
                async);
                Assert.IsNull(next);
                targetForm.Close(new Async());
            }
        }

        /// <summary>
        /// クリックエミュレート
        /// </summary>
        /// <param name="c">コントロール</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        static void Click(WindowControl c, int x, int y)
        {
            uint pos = MousePointToNative(x, y);
            c.SetFocus();
            c.SequentialMessage(
                new MessageInfo(0x0201, 0x0001, pos),
                new MessageInfo(0x0202, 0x0000, pos));
        }

        /// <summary>
        /// クリックエミュレート
        /// </summary>
        /// <param name="c">コントロール</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="async">非同期オブジェクト</param>
        static void Click(WindowControl c, int x, int y, out Async async)
        {
            async = new Async();
            uint pos = MousePointToNative(x, y);
            c.SetFocus();
            c.SequentialMessage(async,
                new MessageInfo(0x0201, 0x0001, pos),
                new MessageInfo(0x0202, 0x0000, pos));
        }

        /// <summary>
        /// マウス座標をネイティブ送信用に変換。
        /// </summary>
        /// <param name="x">X座標。</param>
        /// <param name="y">Y座標。</param>
        /// <returns>uint値。</returns>
        static uint MousePointToNative(int x, int y)
        {
            return ((uint)y << 16) + (uint)x;
        }

        /// <summary>
        /// longからIntPtrに変換
        /// </summary>
        /// <param name="value">long値</param>
        /// <returns>IntPtr値</returns>
        static IntPtr LongToIntPtr(long value)
        {
            if (IntPtr.Size == 4)
            {
                uint tmp = (uint)value;
                return new IntPtr((int)tmp);
            }
            return new IntPtr(value);
        }
    }
}
