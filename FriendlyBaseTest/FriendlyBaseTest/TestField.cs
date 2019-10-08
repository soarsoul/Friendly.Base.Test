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
    /// Field
    /// </summary>
    [TestFixture]
    public class TestField
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
        /// フィールド操作に関するテスト
        /// </summary>
        [Test]
        public void Test()
        {
            //publicメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["_publicIntValue"](1);
                Assert.AreEqual((int)v["_publicIntValue"]().Core, 1);
            }
            //privateメンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["_privateIntValue"](2);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 2);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 0);
            }
            //privateメンバで隠された親メンバ
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["_privateIntValue", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)v["ChildIntValue"]().Core, 0);
                Assert.AreEqual((int)v["ParentIntValue"]().Core, 3);
            }
        }

        /// <summary>
        /// staticフィールド操作に関するテスト
        /// </summary>
        [Test]
        public void TestStatic()
        {
            //publicメンバ
            {
                app[typeof(OperationTestClassPublic), "_publicIntValueStatic"](1);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "_publicIntValueStatic"]().Core, 1);
            }
            //privateメンバ
            {
                app[typeof(OperationTestClassPublic), "_privateIntValueStatic"](2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 0);
            }
            //privateメンバでnewによって隠された親メンバ
            {
                app[typeof(OperationTestClassBasePublic), "_privateIntValueStatic", new OperationTypeInfo(typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName)](3);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ChildIntValueStatic"]().Core, 2);
                Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "ParentIntValueStatic"]().Core, 3);
            }
        }

        /// <summary>
        /// staticフィールド操作に関するテスト
        /// staticの呼び出しはいくつかのスタイルがある。
        /// </summary>
        [Test]
        public void TestStaticManyStyle()
        {
            //Type,operation
            app[typeof(OperationTestClassPublic), "_publicIntValueStatic"](1);
            Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "_publicIntValueStatic"]().Core, 1);

            //fullOperation
            app["FriendlyBaseTargetNet20.OperationTestClassPublic._publicIntValueStatic"](3);
            Assert.AreEqual((int)app["FriendlyBaseTargetNet20.OperationTestClassPublic._publicIntValueStatic"]().Core, 3);
        }

        /// <summary>
        /// フィールド操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestAsync()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
            Async async = new Async();
            v["_publicIntValue", async](1);
            while (!async.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Assert.AreEqual((int)v["_publicIntValue"]().Core, 1);
        }

        /// <summary>
        /// staticフィールド操作に関するテスト
        /// 非同期
        /// </summary>
        [Test]
        public void TestStaticAsync()
        {
            Async async = new Async();
            app[typeof(OperationTestClassPublic), "_publicIntValueStatic", async](100);
            while (!async.IsCompleted)
            {
                Thread.Sleep(1);
            }
            Assert.AreEqual((int)app[typeof(OperationTestClassPublic), "_publicIntValueStatic"]().Core, 100);
        }

        /// <summary>
        /// AppVarでの値セット
        /// </summary>
        [Test]
        public void TestAppVarSet()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
            v["_publicIntValue"](app.Dim(5));
            Assert.AreEqual((int)v["_publicIntValue"]().Core, 5);
        }
         
        /// <summary>
        /// 例外に関するテスト
        /// </summary>
        [Test]
        public void TestException()
        {
            AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());

            //間違った個数の引数の引き渡し
            try
            {
                v["_publicIntValue"](5, 1);
                Assert.Fail();
            }
            catch (FriendlyOperationException e)
            {
                string message = "[型 : OperationTestClassPublic][操作 : _publicIntValue(Int32, Int32)]" + Environment.NewLine +
                            "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                            "引数指定が間違っている可能性があります。" + Environment.NewLine +
                            "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                            "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                            "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(message, e.Message);
            }
            
            //値型に対してnull指定
            try
            {
                v["_publicIntValue"](null);
                Assert.Fail();
            }
            catch (FriendlyOperationException e)
            {
                string message = "[型 : OperationTestClassPublic][操作 : _publicIntValue(null)]" + Environment.NewLine +
                            "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                            "引数指定が間違っている可能性があります。" + Environment.NewLine +
                            "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                            "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                            "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(e.Message, message);
            }

            //間違った種類の引数引き渡し
            try
            {
                v["_publicIntValue"](new object());
                Assert.Fail();
            }
            catch (FriendlyOperationException e)
            {
                string message = "[型 : OperationTestClassPublic][操作 : _publicIntValue(Object)]" + Environment.NewLine +
                            "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                            "引数指定が間違っている可能性があります。" + Environment.NewLine +
                            "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                            "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                            "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// Nullableのテスト
        /// </summary>
        [Test]
        public void TestNullable()
        {
            app[typeof(OperationTestClassPublic), "_nullableValue"](1);
            int? value = (int?)app[typeof(OperationTestClassPublic), "_nullableValue"]().Core;
            Assert.AreEqual(1, value.Value);

            app[typeof(OperationTestClassPublic), "_nullableValue"](null);
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
