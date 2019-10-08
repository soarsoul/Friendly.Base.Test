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
    /// Property
    /// </summary>
    [TestFixture]
    public class TestProperty
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
        /// []のテスト
        /// </summary>
        [Test]
        public void TestGetItem()
        {
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[]"](1, 1);
                Assert.AreEqual((int)v["[]"](1).Core, 1);
            }
            //privateメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[]"](2, 2);
                Assert.AreEqual((int)v["[]"](2).Core, 2);
            }
            //privateメンバでnewによって隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[]"](2, 2);
                v["[]", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName, typeof(int).FullName)](3, 3);
                Assert.AreEqual((int)v["[]"](2).Core, 2);
                Assert.AreEqual((int)v["[]", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3).Core, 3);
            }
            //インデックスが2つあるget_Item
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[,]"]("3", 2, 6);
                Assert.AreEqual((int)v["[,]"]("3", 2).Core, 6);
            }

            //get_Item, set_Itemでもアクセス
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["set_Item"]("3", 2, 6);
                Assert.AreEqual((int)v["get_Item"]("3", 2).Core, 6);
            }

            //ボタンとフォームで切り替え
            {
                AppVar button = app.Dim(new NewInfo<Button>());
                AppVar form = app.Dim(new NewInfo<Form>());
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[]"](button, 2);
                v["[]"](form, 3);
                Assert.AreEqual((int)v["[]"](button).Core, 2);
                Assert.AreEqual((int)v["[]"](form).Core, 3);
            }

            //引数を型情報で解決
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["[]", new OperationTypeInfo(typeof(OperationTestClassPublic).FullName, typeof(Button).FullName, typeof(int).FullName)](null, 2);
                v["[]", new OperationTypeInfo(typeof(OperationTestClassPublic).FullName, typeof(Form).FullName, typeof(int).FullName)](null, 3);
                Assert.AreEqual((int)v["[]", new OperationTypeInfo(typeof(OperationTestClassPublic).FullName, typeof(Button).FullName)](null).Core, 2);
                Assert.AreEqual((int)v["[]", new OperationTypeInfo(typeof(OperationTestClassPublic).FullName, typeof(Form).FullName)](null).Core, 3);
            }
        }

        /// <summary>
        /// プロパティー操作に関するテスト
        /// </summary>
        [Test]
        public void Test()
        {
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["PublicIntValue"](1);
                Assert.AreEqual((int)v["PublicIntValue"]().Core, 1);
            }
            //privateメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["PrivateIntValue"](2);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 2);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 0);
            }
            //privateメンバでnewによって隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["PrivateIntValue", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 0);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 3);
            }
        }

        /// <summary>
        /// staticプロパティー操作に関するテスト
        /// </summary>
        [Test]
        public void TestStatic()
        {
            //publicメンバ
            {
                app[typeof(OperationTestClassPublic), "PublicIntValueStatic"](1);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 1);
            }
            //privateメンバ
            {
                app[typeof(OperationTestClassPublic), "PrivateIntValueStatic"](2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 0);
            }
            //privateメンバでnewによって隠された親メンバ
            {
                app[typeof(OperationTestClassBasePublic), "PrivateIntValueStatic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }
        }

        /// <summary>
        /// staticプロパティー操作に関するテスト
        /// staticは様々な呼び出しがある
        /// </summary>
        [Test]
        public void TestStaticManyStyle()
        {
            //Type,operation
            app[typeof(OperationTestClassPublic), "PublicIntValueStatic"](1);
            Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "PublicIntValueStatic"]().Core, 1);

            //fullOperation
            app["FriendlyBaseTargetNet20.OperationTestClassPublic.PublicIntValueStatic"](3);
            Assert.AreEqual((int)app["FriendlyBaseTargetNet20.OperationTestClassPublic.PublicIntValueStatic"]().Core, 3);
        }

        /// <summary>
        /// プロパティー操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestAsync()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
            Async async = new Async();
            v["PublicIntValue", async](1);
            while (!async.IsCompleted)
            {
                Thread.Sleep(10);
            }
            Assert.AreEqual((int)v["PublicIntValue"]().Core, 1);
        }

        /// <summary>
        /// staticプロパティー操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestStaticAsync()
        {
            Async async = new Async();
            app[typeof(OperationTestClassPublic), "PublicIntValueStatic", async](100);
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
            v["PublicIntValue"](app.Dim(5));
            Assert.AreEqual((int)v["PublicIntValue"]().Core, 5);
        }

        /// <summary>
        /// 配列のテスト
        /// </summary>
        [Test]
        public void TestArray()
        {
            AppVar v = app.Dim(new int[10]);
            v["[]"](1, 100);
            Assert.AreEqual((int)v["[]"](1).Core, 100);
        }

        /// <summary>
        /// 多次元配列のテスト
        /// </summary>
        [Test]
        public void TestMultidimensionArray()
        {
            AppVar v = app.Dim(new int[][] { new int[10], new int[10] });
            v["[]"](1)["[]"](1, 100);
            Assert.AreEqual((int)v["[]"](1)["[]"](1).Core, 100);
        }

        /// <summary>
        /// ジャグ配列のテスト
        /// </summary>
        [Test]
        public void TestJugArray()
        {
            AppVar v = app.Dim(new int[10, 10]);
            v["[,]"](1, 1, 100);
            Assert.AreEqual((int)v["[,]"](1, 1).Core, 100);
        }

        /// <summary>
        /// Nullableのテスト
        /// </summary>
        [Test]
        public void TestNullable()
        {
            app[typeof(OperationTestClassPublic), "NullableValue"](1);
            int? value = (int?)app[typeof(OperationTestClassPublic), "_nullableValue"]().Core;
            Assert.AreEqual(1, value.Value);

            app[typeof(OperationTestClassPublic), "NullableValue"](null);
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
