using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// オブジェクト引数テスト用
    /// </summary>
    [Serializable]
    public class ObjectArgsTarget
    {
        public object[] _field;

        object[] _propertyCore;
        public object[] Property { get { return _propertyCore; } set { _propertyCore = value; } }

        public bool _callObjArray;
        public void Method(object[] val)
        {
            _callObjArray = true;
        }

        public bool _callInt;
        public void Method(int i, int ii)
        {
            _callInt = true;
        }

        public bool _callObj;
        public void Method(object o)
        {
            _callObj = true;
        }

        public ObjectArgsTarget() { }

        public bool _constructorObjArray;
        public ObjectArgsTarget(object[] val)
        {
            _constructorObjArray = true;
        }

        public bool _constructorInt;
        public ObjectArgsTarget(int i, int ii)
        {
            _constructorInt = true;
        }

        public bool _constructorObj;
        public ObjectArgsTarget(object o)
        {
            _constructorObj = true;
        }

        public string arrayFirst;
        public void Method(params string[] array)
        {
            arrayFirst = array[0];
        }
    }
}
