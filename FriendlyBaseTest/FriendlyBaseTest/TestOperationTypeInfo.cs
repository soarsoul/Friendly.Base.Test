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
    /// OperationTypeInfoに関するテスト
    /// </summary>
    [TestFixture]
    public class TestOperationTypeInfo
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
        /// 正常にシリアライズできるテスト
        /// </summary>
        [Test]
        public void TestValid()
        {
            //正しく使用することができた
            AppVar v = app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(typeof(DimTargetPublic).FullName, typeof(Control).FullName));
            Assert.AreEqual(((DimTargetPublic)v.Core).IntValue, 10);
        }

        /// <summary>
        /// 引数例外テスト
        /// </summary>
        [Test]
        public void TestException()
        {
            try
            {
                app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(null));
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                string message = "値を Null にすることはできません。" + Environment.NewLine + "パラメーター名:target";
                Assert.AreEqual(e.Message, message);
            }

            try
            {
                app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(GetType().FullName, null));
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                string message = "値を Null にすることはできません。" + Environment.NewLine + "パラメーター名:arguments";
                Assert.AreEqual(e.Message, message);
            }

            try
            {
                app.Dim(new NewInfo<DimTargetPublic>(null), new OperationTypeInfo(GetType().FullName, GetType().FullName, null));
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                string message = "arguments is invalid";
                Assert.AreEqual(e.Message, message);
            }

            try
            {
                AppVar v = app.Dim(new NewInfo<OperationTestClassPublic>());
                v["SetPrivateIntValueMethodPublic", new OperationTypeInfo(
                    typeof(OperationTestClassBasePublic).FullName, typeof(int).FullName, typeof(int).FullName)](3);
                Assert.IsTrue(false);
            }
            catch (FriendlyOperationException e)
            {
                string message = "[OperationTypeInfo.Arguments : (Int32, Int32)][引数 : (Int32)]" + Environment.NewLine +
                                "操作型情報の引数指定が不正です。実際に引き渡している引数の数と型指定の数が一致しません。" + Environment.NewLine +
                                "params付きの配列の場合は、その型の配列に格納して渡してください。";

                Assert.AreEqual(e.Message, message);
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
    }
}
