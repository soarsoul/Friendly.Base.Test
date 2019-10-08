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
    public class TestDim
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
        /// publicコンストラクタのテスト
        /// </summary>
        [Test]
        public void TestDimPublic()
        {
            //生成 デフォルトコンストラクタ
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>());
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 0);
            }
            //生成 引数付コンストラクタ
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>(1));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 1);
            }
            //初期値代入
            {
                AppVar v = app.Dim(new DimTargetPublic(2));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 2);
            }
        }

        /// <summary>
        /// privateコンストラクタのテスト
        /// </summary>
        [Test]
        public void TestDimPrivate()
        {
            //生成 デフォルトコンストラクタ
            {
                AppVar v = app.Dim(new NewInfo("FriendlyBaseTargetNet20.DimTargetInternal"));
                AppVar intValue = v["_intValue"]();
                Assert.AreEqual((int)intValue.Core, 0);
            }
            //生成 引数付コンストラクタ
            {
                AppVar v = app.Dim(new NewInfo("FriendlyBaseTargetNet20.DimTargetInternal", 1));
                AppVar intValue = v["_intValue"]();
                Assert.AreEqual((int)intValue.Core, 1);
            }
        }

        /// <summary>
        /// protectedコンストラクタのテスト
        /// </summary>
        [Test]
        public void TestDimProtected()
        {
            //生成
            {
                AppVar v = app.Dim(new NewInfo("FriendlyBaseTargetNet20.DimTargetInternal", (long)3));
                AppVar intValue = v["_intValue"]();
                Assert.AreEqual((int)intValue.Core, 30);
            }
        }
        
        /// <summary>
        /// オーバーロード解決
        /// </summary>
        [Test]
        public void TestDimOverload()
        {
            //オーバーロード解決不可能例外が出ることを確認
            try
            {
                app.Dim(new NewInfo<DimTargetPublic>(null));
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[new DimTargetPublic(null)]" + Environment.NewLine +
                                "指定の引数で実行できる可能性のあるコンストラクタが複数発見されました。" + Environment.NewLine + 
                                "引数に引き渡す型を明確にするか、もしくはOperationTypeInfoを使用してください。";
                Assert.AreEqual(e.Message, message);
            }
            //引数の型でオーバーロード解決
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>(app.Dim(new NewInfo<Control>())));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 10);
            }
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>(app.Dim(new NewInfo<Form>())));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 100);
            }
            //指定でオーバーロード解決
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(typeof(DimTargetPublic).FullName, typeof(Control).FullName));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 10);
            }
            {
                AppVar v = app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(typeof(DimTargetPublic).FullName, typeof(Form).FullName));
                Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 100);
            }
        }
       
        /// <summary>
        /// AppVarでの値セット
        /// </summary>
        [Test]
        public void TestAppVarSet()
        {
            AppVar v = app.Dim(new NewInfo<DimTargetPublic>(app.Dim(5)));
            Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 5);
        }

        /// <summary>
        /// out, ref解決
        /// </summary>
        [Test]
        public void TestOutRef()
        {
            {
                AppVar arg1 = app.Dim();
                AppVar arg2 = app.Dim();
                AppVar v = app.Dim(new NewInfo<OutRef>(arg1, arg2));
                Assert.AreEqual((string)arg1.Core, "outValue");
                Assert.AreEqual((string)arg2.Core, "refValue");
            }

            //out,refの値は取得できないが生成は可能であること
            {
                AppVar v = app.Dim(new NewInfo<OutRef>(null, null));
                Assert.AreEqual((string)v["GetType"]()["FullName"]().Core, typeof(OutRef).FullName);
            }

            //オーバーロード解決不可能例外が出ることを確認
            try
            {
                app.Dim(new NewInfo<OutRef>(app.Dim()));
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[new OutRef(null)]" + Environment.NewLine +
                                "指定の引数で実行できる可能性のあるコンストラクタが複数発見されました。" + Environment.NewLine +
                                "引数に引き渡す型を明確にするか、もしくはOperationTypeInfoを使用してください。";
                Assert.AreEqual(e.Message, message);
            }

            //指定でオーバーロード解決
            {
                AppVar arg = app.Dim();
                AppVar v = app.Dim(new NewInfo<OutRef>(arg), new OperationTypeInfo(typeof(OutRef).FullName, typeof(Control).FullName));
                Assert.AreEqual((string)v["GetType"]()["FullName"]().Core, typeof(OutRef).FullName);
                Assert.AreEqual((string)v["_constructorResult"]()["GetType"]()["FullName"]().Core, typeof(DataGrid).FullName);
            }
            {
                AppVar arg = app.Dim();
                AppVar v = app.Dim(new NewInfo<OutRef>(arg), new OperationTypeInfo(typeof(OutRef).FullName, typeof(Control).FullName + "&"));
                Assert.AreEqual((string)v["GetType"]()["FullName"]().Core, typeof(OutRef).FullName);
                Assert.AreEqual((string)arg["GetType"]()["FullName"]().Core, typeof(Button).FullName);
            }
            {
                AppVar arg = app.Dim();
                AppVar v = app.Dim(new NewInfo<OutRef>(arg), new OperationTypeInfo(typeof(OutRef).FullName, typeof(Form).FullName + "&"));
                Assert.AreEqual((string)v["GetType"]()["FullName"]().Core, typeof(OutRef).FullName);
                Assert.AreEqual((string)arg["GetType"]()["FullName"]().Core, typeof(Form).FullName);
            }
        }

        /// <summary>
        /// 未発見例外テスト
        /// </summary>
        [Test]
        public void TestDimNotFoundException()
        {
            try
            {
                app.Dim(new NewInfo("FriendlyBaseTargetNet20.XXX"));
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[FriendlyBaseTargetNet20.XXX]" + Environment.NewLine +
                                    "指定の型が見つかりません。" + Environment.NewLine +
                                    "型フルネームが間違っているか、指定の型を含むモジュールが、まだロードされていない可能性があります。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// インターフェイスの生成例外テスト
        /// </summary>
        [Test]
        public void TestDimInterface()
        {
            try
            {
                app.Dim(new NewInfo(typeof(InterfacePublic)));
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[new InterfacePublic()]" + Environment.NewLine +
                                "コンストラクタが見つかりませんでした。" + Environment.NewLine +
                                "引数指定が間違っている可能性があります。" + Environment.NewLine +
                                "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                                "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                                "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// 抽象クラスの生成例外テスト
        /// </summary>
        [Test]
        public void TestDimAbstract()
        {
            try
            {
                app.Dim(new NewInfo(typeof(AbstractClassPublic)));
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string coreMessage = "抽象クラスであるため、FriendlyBaseTargetNet20.AbstractClassPublic のインスタンスを作成できません。";
                Assert.IsTrue(e.Message.IndexOf(coreMessage) != -1);
            }
        }

        /// <summary>
        /// 抽象クラスの生成例外テスト
        /// </summary>
        [Test]
        public void TestDimNul()
        {
            {
                AppVar v = app.Dim();
                Assert.IsTrue(v.Core == null);
            }
            {
                AppVar v = app.Dim(null);
                Assert.IsTrue(v.Core == null);
            }
            {
                //念のため。NewInfoでもnullなら空の変数が宣言される使用
                AppVar v = app.Dim((NewInfo)null);
                Assert.IsTrue(v.Core == null);
            }
            {
                AppVar v = app.Dim(new NewInfo<DimTargetNullObjectPublic>(null));
                AppVar val = v["_intValue"]();
                Assert.AreEqual((int)val.Core, 100);
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
                AppVar v = app.Dim(new NewInfo("FriendlyBaseTargetNet20.DimTargetInternal", null));
                Assert.Fail();
            }
            catch (FriendlyOperationException e)
            {
                string message = "[new DimTargetInternal(null)]" + Environment.NewLine +
                                               "コンストラクタが見つかりませんでした。" + Environment.NewLine +
                                               "引数指定が間違っている可能性があります。" + Environment.NewLine +
                                               "数値型、Enumも厳密に判定されるのでご注意お願いします。" + Environment.NewLine +
                                               "(例として、long型の引数にint型を渡しても別の型と判断され、解決に失敗します。)" + Environment.NewLine +
                                               "また、params付きの配列の場合は、その型の配列に格納して渡してください。";
                Assert.AreEqual(e.Message, message);
            }

            //out,refは正しく呼び出せること
            {
                AppVar v = app.Dim(new NewInfo<OutRef>(null, null));
                Assert.IsTrue((bool)v["_isConstructorString"]().Core);
            }
        }

        /// <summary>
        /// 値型の生成
        /// </summary>
        [Test]
        public void TestValueType()
        {
            //int値
            AppVar intVal = app.Dim(new NewInfo<int>());
            Assert.AreEqual(typeof(int), (Type)intVal["GetType"]().Core);
        
            //Point
            AppVar posVal = app.Dim(new NewInfo<Point>());
            posVal["X"](100);
            Assert.AreEqual(100, (int)posVal["X"]().Core);

            AppVar posVal2 = app.Dim(new NewInfo<Point>(200, 100));
            Assert.AreEqual(200, (int)posVal2["X"]().Core);
            Assert.AreEqual(100, (int)posVal2["Y"]().Core);
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