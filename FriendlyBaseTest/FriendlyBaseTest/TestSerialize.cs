using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NUnit.Framework;
using Codeer.Friendly;
using Codeer.Friendly.Inside.Protocol;

namespace FriendlyBaseTest
{
    /// <summary>
    /// シリアライズテスト
    /// </summary>
    [TestFixture]
    public class TestSerialize
    {
        /// <summary>
        /// FriendlyOperationExceptionのシリアライズテスト
        /// </summary>
        [Test]
        public void TestFriendlyOperationException()
        {
            ExceptionInfo info = null;
            try
            {
                throw new ArgumentException("arg");
            }
            catch (Exception e)
            {
                info = new ExceptionInfo(e);
            }
            FriendlyOperationException exception = new FriendlyOperationException(info);
            IFormatter formatter = new BinaryFormatter();
            byte[] bin = null;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, exception);
                bin = stream.ToArray();
            }
            using (MemoryStream stream = new MemoryStream(bin))
            {
                FriendlyOperationException d = (FriendlyOperationException)formatter.Deserialize(stream);
                Assert.AreEqual(d.ExceptionInfo.ExceptionType, exception.ExceptionInfo.ExceptionType);
                Assert.AreEqual(d.ExceptionInfo.HelpLink, exception.ExceptionInfo.HelpLink);
                Assert.AreEqual(d.ExceptionInfo.Message, exception.ExceptionInfo.Message);
                Assert.AreEqual(d.ExceptionInfo.Source, exception.ExceptionInfo.Source);
                Assert.AreEqual(d.ExceptionInfo.StackTrace, exception.ExceptionInfo.StackTrace);
            }
        }
    }
}
