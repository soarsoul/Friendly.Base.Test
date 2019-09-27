using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using Codeer.Friendly;
using Codeer.Friendly.Windows;
using NUnit.Framework;
using FriendlyBaseTargetNet20;

namespace FriendlyBaseTest
{
    /// <summary>
    /// AppVarのインターフェイスに対する対応
    /// </summary>
    [TestFixture]
    public class TestAppVarInterface
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
        /// AppVarからAppを取得できること
        /// </summary>
        [Test]
        public void TestApp()
        {
            AppVar v = app.Dim();
            Assert.IsTrue(ReferenceEquals(v.App, app));
            v.App[typeof(Application), "OpenForms"]()["[]"](0)["Text"]("abc");
            Assert.AreEqual("abc", v.App[typeof(Application), "OpenForms"]()["[]"](0)["Text"]().Core);
        }

        /// <summary>
        /// Coreプロパティーで正常にシリアライズできるテスト
        /// </summary>
        [Test]
        public void TestCoreSerialize()
        {
            AppVar v = app.Dim(new DimTargetPublic(3));
            Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 3);
        }

        /// <summary>
        /// Coreプロパティーで正常にシリアライズできないテスト
        /// </summary>
        [Test]
        public void TestCoreNonSerialize()
        {
            try
            {
                AppVar v = app.Dim(new NewInfo("FriendlyBaseTargetNet20.NonSerializeObject", 3));
                object o = v.Core;
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

        /// <summary>
        /// nullのオブジェクトに対する操作呼び出し
        /// </summary>
        [Test]
        public void TestNullAppVarCall()
        {
            try
            {
                AppVar v = app.Dim();
                v["Method"]();
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "AppVarの中身がnullのオブジェクトに対して操作を呼び出しました。";
                Assert.AreEqual(e.Message, message);
            }
        }

        /// <summary>
        /// nullの設定と取得
        /// </summary>
        [Test]
        public void TestCoreGetNull()
        {
            AppVar v = app.Dim();
            v.Core = null;
            object val = v.Core;
            Assert.AreEqual(null, val);
        }

        /// <summary>
        /// AppVarの値を入れる
        /// </summary>
        [Test]
        public void TestCoreSetVar()
        {
            AppVar v = app.Dim();
            AppVar vv = app.Dim("abc");
            v.Core = vv;
            Assert.AreEqual(v, "abc");
        }

        /// <summary>
        /// Equalsのテスト
        /// </summary>
        [Test]
        public void TestEquals()
        {
            AppVar v1 = app.Dim(1);
            AppVar v2 = app.Dim(1);
            AppVar v3 = app.Dim(3);
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsFalse(v1.Equals(v3));
        }

        /// <summary>
        /// ToStringのテスト
        /// </summary>
        [Test]
        public void TestToString()
        {
            AppVar v = app.Dim(5);
            Assert.AreEqual(v.ToString(), "5");
        }

        /// <summary>
        /// GetHashCodeのテスト
        /// </summary>
        [Test]
        public void TestGetHashCode()
        {
            AppVar v = app.Dim("abc");
            Assert.AreEqual(v.GetHashCode(), "abc".GetHashCode());
        }

        /// <summary>
        /// Disposeのテスト
        /// Dispose後にアクセスした場合、例外が発生する。
        /// </summary>
        [Test]
        public void TestDispose()
        {
            AppVar v = app.Dim("abc");
            v.Dispose();
            try
            {
                v["Length"]();
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                Assert.AreEqual(e.Message, "既に破棄されたオブジェクトです。");
            }
        }

        /// <summary>
        /// 正常にforeachが利用できるテスト
        /// </summary>
        [Test]
        public void TestForeachOK()
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            AppVar v = app.Dim(list);
            int sum = 0;
            foreach (AppVar element in new Enumerate(v))
            {
                sum += (int)element.Core;
            }
            Assert.AreEqual(sum, 6);
        }

        /// <summary>
        /// foreachが利用できないオブジェクトのテスト
        /// </summary>
        [Test]
        public void TestForeachNG()
        {
            AppVar v = app.Dim(3);
            try
            {
                foreach (AppVar element in new Enumerate(v)) { }
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "指定の変数はIEnumerableを実装していません。";
                Assert.AreEqual(e.Message, message);
            }
        }

        [Test]
        public void TestIsNull()
        {
            AppVar var = app.Dim();
            Assert.IsTrue(var.IsNull);
            var.Core = 1;
            Assert.IsFalse(var.IsNull);
            AppVar form = app.Dim(new NewInfo<Form>());
            Assert.IsFalse(form.IsNull);
            Assert.IsTrue(form["Parent"]().IsNull);
        }


        [Test]
        public void TestHelperIsNull()
        {
            AppVar var = app.Dim();
            Assert.IsTrue(AppVarHelper.IsNull(var));
            var.Core = 1;
            Assert.IsFalse(AppVarHelper.IsNull(var));
            AppVar form = app.Dim(new NewInfo<Form>());
            Assert.IsFalse(AppVarHelper.IsNull(form));
            Assert.IsTrue(AppVarHelper.IsNull(form["Parent"]()));
        }

        [Test]
        public void TestHelperReferenceEquals()
        {
            AppVar main1 = app[typeof(Application), "OpenForms"]()["[]"](0);
            AppVar main2 = app[typeof(Control), "FromHandle"](Process.GetProcessById(app.ProcessId).MainWindowHandle);
            AppVar form = app.Dim(new NewInfo<Form>());

            Assert.IsTrue(AppVarHelper.ReferenceEquals(main1, main2));
            Assert.IsFalse(AppVarHelper.ReferenceEquals(main1, form));

            Assert.IsFalse(AppVarHelper.ReferenceEquals(1, 1));
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

