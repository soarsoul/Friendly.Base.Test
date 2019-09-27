using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// 操作対象クラス
    /// </summary>
    [Serializable]
    public class OperationTestClassPublic : OperationTestClassBasePublic
    {
        public int _publicIntValue;
        public static int _publicIntValueStatic;
        int _privateIntValue;
        static int _privateIntValueStatic;
        int _buttonValue;
        int _formValue;
        Dictionary<int, int> _dicIntInt = new Dictionary<int, int>();
        Dictionary<string, int> _dicStringInt = new Dictionary<string, int>();

        public int PublicIntValue { get { return _publicIntValue; } set { _publicIntValue = value; } }
        
        public static int PublicIntValueStatic { get { return _publicIntValueStatic; } set { _publicIntValueStatic = value; } }

        internal new int PrivateIntValue { get { return _privateIntValue; } set { _privateIntValue = value; } }

        internal static new int PrivateIntValueStatic { get { return _privateIntValueStatic; } set { _privateIntValueStatic = value; } }

        internal int ChildIntValue { get { return _privateIntValue; } set { _privateIntValue = value; } }


        internal static int ChildIntValueStatic { get { return _privateIntValueStatic; } set { _privateIntValueStatic = value; } }

        int this[int index] { get { return _dicIntInt[index]; } set { _dicIntInt.Add(index, value); } }

        int this[string index] { get { return _dicStringInt[index]; } set { _dicStringInt.Add(index, value); } }

        int this[string index, int index2] { get { return _dicIntInt[int.Parse(index) + index2]; } set { _dicIntInt.Add(int.Parse(index) + index2, value); } }

        int this[Button b] { get { return _buttonValue; } set { _buttonValue = value; } }

        int this[Form f] { get { return _formValue; } set { _formValue = value; } }

        private void SetPrivateIntValueMethodPrivate(int value)
        {
            _privateIntValue = value;
        }

        private static void SetPrivateIntValueStaticMethodPrivate(int value)
        {
            _privateIntValueStatic = value;
        }

        public new void SetPrivateIntValueMethodPublic(int value)
        {
            _privateIntValue = value;
        }

        public new static void SetPrivateIntValueStaticMethodPublic(int value)
        {
            _privateIntValueStatic = value;
        }

        public void SetPublicIntValueMethodPublic(int value)
        {
            _publicIntValue = value;
        }

        public static void SetPublicIntValueStaticMethodPublic(int value)
        {
            _publicIntValueStatic = value;
        }

        public static void ThrowException()
        {
            throw new SystemException("test");
        }

        static int? _nullableValue;
        static int? NullableValue { get { return _nullableValue; } set { _nullableValue = value; } }
        public static void NullableTest(int? value)
        {
            _nullableValue = value;
        }
    }
}
