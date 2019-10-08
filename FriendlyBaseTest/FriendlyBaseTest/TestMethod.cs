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

namespace FriendlyBaseTest
{
    /// <summary>
    /// AppVarの操作に対する検査
    /// Method
    /// </summary>
    [TestFixture]
    public class TestMethod
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
        /// プメソッド操作に関するテスト
        /// </summary>
        [Test]
        public void Test()
        {
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPublicIntValueMethodPublic"](1);
                Assert.AreEqual((int)v["PublicIntValue"]().Core, 1);
            }
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPrivateIntValueMethodPublic"](2);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 2);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 0);
            }
            //publicメンバで隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPrivateIntValueMethodPublic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 0);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 3);
            }
            //privateメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPrivateIntValueMethodPrivate"](4);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 4);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 0);
            }
            //privateメンバで隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPrivateIntValueMethodPrivate", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](5);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 0);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 5);
            }
        }

        /// <summary>
        /// staticメソッド操作に関するテスト
        /// </summary>
        [Test]
        public void TestStatic()
        {
            //publicメンバ
            {
                app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic"](1);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 1);
            }

            //publicメンバ
            {
                app[typeof(OperationTestClassPublic), "SetPrivateIntValueStaticMethodPublic"](2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 0);
            }
            //publicメンバで隠された親メンバ
            {
                app[typeof(OperationTestClassBasePublic), "SetPrivateIntValueStaticMethodPublic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }

            //privateメンバ
            {
                app[typeof(OperationTestClassPublic), "SetPrivateIntValueStaticMethodPrivate"](4);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 4);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }
            //privateメンバで隠された親メンバ
            {
                app[typeof(OperationTestClassBasePublic), "SetPrivateIntValueStaticMethodPrivate", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](5);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 4);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 5);
            }
        }

        /// <summary>
        /// staticメソッド操作に関するテスト
        /// staticは様々な呼び出しスタイルがある
        /// </summary>
        [Test]
        public void TestStaticManyStyle()
        {
            //Type,operation
            app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic"](1);
            Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 1);

            //fullOperation
            app["FriendlyBaseTargetNet20.OperationTestClassPublic.SetPublicIntValueStaticMethodPublic"](3);
            Assert.AreEqual((int)app["FriendlyBaseTargetNet20.OperationTestClassPublic.PublicIntValueStatic"]().Core, 3);
        }

        /// <summary>
        /// In,Out修飾があっても正常に呼び出しができることの確認
        /// </summary>
        [Test]
        public void TestMarshalModify()
        {
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestIn"](1).Core, 1);
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestOut"](2).Core, 2);
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestInOut"](3).Core, 3);
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestInRef"](4).Core, 4);
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestInOutRef"](5).Core, 5);
            Assert.AreEqual((int)app[typeof(MarshalModify), "TestOutOut"](0).Core, 100);
        }

        /// <summary>
        /// 内部的に発生した例外を取得するテスト
        /// </summary>
        [Test]
        public void TestException()
        {
            try
            {
                app[typeof(ExceptionTest), "ThrowException"]();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.IndexOf("CheckExceptionMessage") != -1);
                Assert.IsTrue(e.Message.IndexOf("対象アプリケーション内部で例外が発生しました。") != -1);
                return;
            }
            Assert.Fail();
        }

        /// <summary>
        /// メソッド操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestAsync()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
            Async async = new Async();
            v["SetPublicIntValueMethodPublic", async](1);
            while (!async.IsCompleted)
            {
                Thread.Sleep(10);
            }
            Assert.AreEqual((int)v["PublicIntValue"]().Core, 1);
        }

        /// <summary>
        /// staticメソッド操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestStaticAsync()
        {
            Async async = new Async();
            app[typeof(OperationTestClassPublic), "SetPublicIntValueStaticMethodPublic", async](100);
            while (!async.IsCompleted)
            {
                Thread.Sleep(10);
            }
            Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 100);
        }

        /// <summary>
        /// AppVarでの値セット
        /// </summary>
        [Test]
        public void TestAppVarSet()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
            v["SetPublicIntValueMethodPublic"](app.Dim(5));
            Assert.AreEqual((int)v["PublicIntValue"]().Core, 5);
        }

        /// <summary>
        /// out, ref解決兼オーバーロード解決テスト
        /// </summary>
        [Test]
        public void TestOutRefAndOverload()
        {
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                AppVar arg1 = app.Dim();
                AppVar arg2 = app.Dim();
                v["OutRefMethod"](arg1, arg2);
                Assert.AreEqual((string)arg1.Core, "outValue");
                Assert.AreEqual((string)arg2.Core, "refValue");
            }

            //out,refの値は取得できないが呼び出しは可能であること
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                AppVar ret = v["OutRefMethod"](null, null);
                Assert.AreEqual((int)ret.Core, 100);
            }

            //オーバーロード解決不可能例外が出ることを確認
            try
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                v["CheckOverload"](null);
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[型 : OutRef][操作 : CheckOverload(null)]" + Environment.NewLine +
                                "指定の引数で実行できる可能性のある操作が複数発見されました。" + Environment.NewLine +
                                "引数に引き渡す型を明確にするか、もしくはOperationTypeInfoを使用してください。";
                Assert.AreEqual(e.Message, message);
            }

            //指定でオーバーロード解決
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                AppVar arg = app.Dim();
                int result = (int)v["CheckOverload", new OperationTypeInfo(typeof(OutRef).FullName, typeof(Control).FullName)](arg).Core;
                Assert.AreEqual(10, result);
            }
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                AppVar arg = app.Dim();
                v["CheckOverload", new OperationTypeInfo(typeof(OutRef).FullName, typeof(Control).FullName + "&")](arg);
                Assert.AreEqual((string)arg["GetType"]()["FullName"]().Core, typeof(Button).FullName);
            }
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                AppVar arg = app.Dim();
                v["CheckOverload", new OperationTypeInfo(typeof(OutRef).FullName, typeof(Form).FullName + "&")](arg);
                Assert.AreEqual((string)arg["GetType"]()["FullName"]().Core, typeof(Form).FullName);
            }
        }

        /// <summary>
        /// 値型に対してnull指定
        /// </summary>
        [Test]
        public void TestNullArg()
        {
            try
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPublicIntValueMethodPublic"](null);
                Assert.Fail();
            }
            catch (FriendlyOperationException e)
            {
                string message = "[型 : OperationTestClassPublic][操作 : SetPublicIntValueMethodPublic(null)]" + Environment.NewLine +
                            "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                            "引数指定が間違っている可能性があります。" + Environment.NewLine +
                            "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                            "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                            "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(e.Message, message);
            }

            //out,refは正しく呼び出せること
            {
                AppVar v = app.Dim(new NewInfo<OutRef>());
                Assert.AreEqual(5, (int)v["OutRefMethodInt"](null, null).Core);
            }
        }

        /// <summary>
        /// Nullableのテスト
        /// </summary>
        [Test]
        public void TestNullable()
        {
            app[typeof(OperationTestClassPublic), "NullableTest"](1);
            int? value = (int?)app[typeof(OperationTestClassPublic), "_nullableValue"]().Core;
            Assert.AreEqual(1, value.Value);

            app[typeof(OperationTestClassPublic), "NullableTest"](null);
            value = (int?)app[typeof(OperationTestClassPublic), "_nullableValue"]().Core;
            Assert.IsNull(value);
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
