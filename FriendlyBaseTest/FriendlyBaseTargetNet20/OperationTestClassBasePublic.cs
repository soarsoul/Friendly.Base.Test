using System.Collections.Generic;
namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// 操作対象クラス基本
    /// </summary>
    public class OperationTestClassBasePublic
    {
        /// <summary>
        /// int値　子クラスにも同様の名称のメンバがある
        /// </summary>
        int _privateIntValue;

        /// <summary>
        /// static int値 子クラスにも同様のメンバがある
        /// </summary>
        static int _privateIntValueStatic;

        /// <summary>
        /// thisプロパティー用
        /// </summary>
        Dictionary<int, int> _dicIntInt = new Dictionary<int, int>();

        /// <summary>
        /// int値　子クラスにも同様の名称のプロパティーがある
        /// </summary>
        internal int PrivateIntValue { get { return _privateIntValue; } set { _privateIntValue = value; } }

        /// <summary>
        /// static int値 子クラスにも同様のプロパティーがある
        /// </summary>
        internal static int PrivateIntValueStatic { get { return _privateIntValueStatic; } set { _privateIntValueStatic = value; } }

        /// <summary>
        /// 名称がぶつからず、int値を操作できる
        /// </summary>
        public int ParentIntValue { get { return _privateIntValue; } set { _privateIntValue = value; } }

        /// <summary>
        /// 名称がぶつからず、staticのint値を操作できる
        /// </summary>
        public static int ParentIntValueStatic { get { return _privateIntValueStatic; } set { _privateIntValueStatic = value; } }

        /// <summary>
        /// thisプロパティーテスト用
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>値</returns>
        int this[int index] { get { return _dicIntInt[index]; } set { _dicIntInt.Add(index, value); } }

        /// <summary>
        /// int値設定　子クラスにも同様のメソッドがある
        /// </summary>
        /// <param name="value">値</param>
        private void SetPrivateIntValueMethodPrivate(int value)
        {
            _privateIntValue = value;
        }

        /// <summary>
        /// static int値設定　子クラスにも同様のメソッドがある
        /// </summary>
        /// <param name="value">値</param>
        private static void SetPrivateIntValueStaticMethodPrivate(int value)
        {
            _privateIntValueStatic = value;
        }

        /// <summary>
        /// int値設定　子クラスにも同様のメソッドがある
        /// </summary>
        /// <param name="value">値</param>
        public void SetPrivateIntValueMethodPublic(int value)
        {
            _privateIntValue = value;
        }

        /// <summary>
        /// static int値設定　子クラスにも同様のメソッドがある
        /// </summary>
        /// <param name="value">値</param>
        public static void SetPrivateIntValueStaticMethodPublic(int value)
        {
            _privateIntValueStatic = value;
        }
    }
}
