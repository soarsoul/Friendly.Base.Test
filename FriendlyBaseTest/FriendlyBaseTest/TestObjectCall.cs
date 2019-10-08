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
    /// 宣言テスト
    /// </summary>
    [TestFixture]
    public class TestObjectCall
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
        /// コンストラクタテスト
        /// </summary>
        [Test]
        public void TestConstructor()
        {
            //普通に呼び出すとintの方が採用される
            {
                AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(1, 2));
                Assert.IsFalse((bool)v["_constructorObjArray"]().Core);
                Assert.IsTrue((bool)v["_constructorInt"]().Core);
                Assert.IsFalse((bool)v["_constructorObj"]().Core);
            }
            {
                AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(new object[] { 1, 2 }));
                Assert.IsFalse((bool)v["_constructorObjArray"]().Core);
                Assert.IsTrue((bool)v["_constructorInt"]().Core);
                Assert.IsFalse((bool)v["_constructorObj"]().Core);
            }

            //見つからない
            {
                try
                {
                    AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(new object[] { 1, 2, 3 }));
                    Assert.Fail();
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[new ObjectArgsTarget(Int32, Int32, Int32)]" + Environment.NewLine +
                       "コンストラクタが見つかりませんでした。" + Environment.NewLine +
                       "引数指定が間違っている可能性があります。" + Environment.NewLine +
                       "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                       "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                       "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                       "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                       "object[] tmpArg = new object[0];" + Environment.NewLine +
                       "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }

            //new object[]で再度ラップしたら呼び出し可能
            {
                AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(new object[] { new object[] { 1, 2 } }));
                Assert.IsTrue((bool)v["_constructorObjArray"]().Core);
                Assert.IsFalse((bool)v["_constructorInt"]().Core);
                Assert.IsFalse((bool)v["_constructorObj"]().Core);
            }

            //型指定で呼び出した場合はマッチする
            {
                AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(null), new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object[]).ToString()));
                Assert.IsTrue((bool)v["_constructorObjArray"]().Core);
                Assert.IsFalse((bool)v["_constructorInt"]().Core);
                Assert.IsFalse((bool)v["_constructorObj"]().Core);
            }
            {
                AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(null), new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object).ToString()));
                Assert.IsFalse((bool)v["_constructorObjArray"]().Core);
                Assert.IsFalse((bool)v["_constructorInt"]().Core);
                Assert.IsTrue((bool)v["_constructorObj"]().Core);
            }

            //指定に合わないのでエラー
            {
                try
                {
                    AppVar v = app.Dim(new NewInfo<ObjectArgsTarget>(new object[] { 1, 2 }), new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object[]).ToString()));
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[OperationTypeInfo.Arguments : (Object[])][引数 : (Int32, Int32)]" + Environment.NewLine + 
                    "操作型情報の引数指定が不正です。実際に引き渡している引数の数と型指定の数が一致しません。" + Environment.NewLine +
                    "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                    "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                    "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                    "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                    "object[] tmpArg = new object[0];" + Environment.NewLine +
                    "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }
        }

        /// <summary>
        /// メソッドテスト
        /// </summary>
        [Test]
        public void TestMethod()
        {
            //普通に呼び出すとintの方が採用される
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method"](1, 2);
                Assert.IsFalse((bool)v["_callObjArray"]().Core);
                Assert.IsTrue((bool)v["_callInt"]().Core);
                Assert.IsFalse((bool)v["_callObj"]().Core);
            }
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method"](new object[] { 1, 2 });
                Assert.IsFalse((bool)v["_callObjArray"]().Core);
                Assert.IsTrue((bool)v["_callInt"]().Core);
                Assert.IsFalse((bool)v["_callObj"]().Core);
            }

            //見つからない
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                try
                {
                    v["Method"](new object[] { 1, 2, 3 });
                    Assert.Fail();
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[型 : ObjectArgsTarget][操作 : Method(Int32, Int32, Int32)]" + Environment.NewLine +
                        "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                        "引数指定が間違っている可能性があります。" + Environment.NewLine +
                       "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                       "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                       "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                       "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                       "object[] tmpArg = new object[0];" + Environment.NewLine +
                       "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }

            //new object[]で再度ラップしたら呼び出し可能
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method"](new object[] { new object[] { 1, 2 } });
                Assert.IsTrue((bool)v["_callObjArray"]().Core);
                Assert.IsFalse((bool)v["_callInt"]().Core);
                Assert.IsFalse((bool)v["_callObj"]().Core);
            }

            //型指定で呼び出した場合はマッチする
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method", new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object[]).ToString())](null);
                Assert.IsTrue((bool)v["_callObjArray"]().Core);
                Assert.IsFalse((bool)v["_callInt"]().Core);
                Assert.IsFalse((bool)v["_callObj"]().Core);
            }
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method", new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object).ToString())](null);
                Assert.IsFalse((bool)v["_callObjArray"]().Core);
                Assert.IsFalse((bool)v["_callInt"]().Core);
                Assert.IsTrue((bool)v["_callObj"]().Core);
            }

            //指定に合わないのでエラー
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                try
                {
                    v["Method", new OperationTypeInfo(typeof(ObjectArgsTarget).ToString(), typeof(object[]).ToString())](new object[] { 1, 2 });
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[OperationTypeInfo.Arguments : (Object[])][引数 : (Int32, Int32)]" + Environment.NewLine +
                    "操作型情報の引数指定が不正です。実際に引き渡している引数の数と型指定の数が一致しません。" + Environment.NewLine +
                    "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                    "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                    "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                    "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                    "object[] tmpArg = new object[0];" + Environment.NewLine +
                    "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }

            //string配列が正しく呼び出せること
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Method"](new string[] { "a", "b", "c" });
                Assert.AreEqual("a", v["arrayFirst"]().ToString());

                try
                {
                    v["Method"]("x", "y", "z");
                    Assert.IsTrue(false);
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[型 : ObjectArgsTarget][操作 : Method(String, String, String)]" + Environment.NewLine +
                        "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                        "引数指定が間違っている可能性があります。" + Environment.NewLine +
                       "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                       "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                       "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                       "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                       "object[] tmpArg = new object[0];" + Environment.NewLine +
                       "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }
        }

        [Test]
        public void TestProperty()
        {
            //見つからない
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                try
                {
                    v["Property"](new object[] { 1, 2 });
                    Assert.Fail();
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual("[型 : ObjectArgsTarget][操作 : Property(Int32, Int32)]" + Environment.NewLine +
                        "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                        "引数指定が間違っている可能性があります。" + Environment.NewLine +
                       "params付きの配列の場合は、その型の配列に格納して渡してください。" + Environment.NewLine +
                       "また、object[] の場合 params object[]と区別がつかず、分解されて引数に渡されます。" + Environment.NewLine +
                       "そのため、object[]の要素にobject[]を入れて引き渡してください。" + Environment.NewLine +
                       "object[] arg;//引き渡したいobject[]" + Environment.NewLine +
                       "object[] tmpArg = new object[0];" + Environment.NewLine +
                       "tmpArg[0] = arg;//これを引き渡してください。", e.Message);
                }
            }

            //正常にget,setが呼び出せる
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Property"](new object[] { new object[] { 1, 2 } });
                AppVar ret = v["Property"]();
                Assert.AreEqual(new object[] { 1, 2 }, (object[])ret.Core);
            }

            //getの方が呼び出される が仕様とする
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["Property"](new object[] { new object[] { 1, 2 } });
                AppVar ret = v["Property"](new object[] {});
                Assert.AreEqual(new object[] { 1, 2 }, (object[])ret.Core);
            }
        }

        /// <summary>
        /// フィールドテスト
        /// </summary>
        [Test]
        public void TestField()
        {
            //見つからない
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                try
                {
                    v["_field"](new object[] { 1, 2 });
                    Assert.Fail();
                }
                catch (FriendlyOperationException e)
                {
                    Assert.AreEqual(
                        "[型 : ObjectArgsTarget][操作 : _field(Int32, Int32)]" + Environment.NewLine +
                        "同名の操作が見つかりましたが、操作を実行できませんでした。" + Environment.NewLine +
                        "引数指定が間違っている可能性があります。" + Environment.NewLine +
                        "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                        "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                        "また、params付きの配列の場合は、その型の配列に格納して渡してください。",
                       e.Message);
                }
            }

            //正常にget,setが呼び出せる
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["_field"](new object[] { new object[] { 1, 2 } });
                AppVar ret = v["_field"]();
                Assert.AreEqual(new object[] { 1, 2 }, (object[])ret.Core);
            }

            //getの方が呼び出される が仕様とする
            {
                AppVar v = app.Dim(new ObjectArgsTarget());
                v["_field"](new object[] { new object[] { 1, 2 } });
                AppVar ret = v["_field"](new object[] { });
                Assert.AreEqual(new object[] { 1, 2 }, (object[])ret.Core);
            }
        }

    }
}
