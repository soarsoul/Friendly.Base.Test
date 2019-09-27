using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// refとoutのテスト用
    /// </summary>
    public class OutRef
    {
        public bool _isConstructorString;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OutRef() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="outValue">out用</param>
        /// <param name="refValue">ref用</param>
        public OutRef(out string outValue, ref string refValue)
        {
            outValue = "outValue";
            refValue = "refValue";
            _isConstructorString = true;
        }

        /// <summary>
        /// 引数解決テスト用
        /// </summary>
        /// <param name="c">コントロール</param>
        public OutRef(out Control c)
        {
            c = new Button();
        }

        Control _constructorResult;

        /// <summary>
        /// 引数解決テスト用
        /// </summary>
        /// <param name="c">コントロール</param>
        public OutRef(Control c)
        {
            _constructorResult = new DataGrid();
        }

        /// <summary>
        /// 引数解決テスト用
        /// </summary>
        /// <param name="f">フォーム</param>
        public OutRef(ref Form f)
        {
            f = new Form();
        }

        /// <summary>
        /// メソッド
        /// </summary>
        /// <param name="outValue">out</param>
        /// <param name="refValue">ref</param>
        /// <returns>100</returns>
        public int OutRefMethod(out string outValue, ref string refValue)
        {
            outValue = "outValue";
            refValue = "refValue";
            return 100;
        }

        /// <summary>
        /// メソッド
        /// </summary>
        /// <param name="outValue">out</param>
        /// <param name="refValue">ref</param>
        public int OutRefMethodInt(out int outValue, ref int refValue)
        {
            outValue = 0;
            refValue = 0;
            return 5;
        }

        /// <summary>
        /// メソッド
        /// </summary>
        /// <param name="c">コントロール</param>
        public void CheckOverload(out Control c)
        {
            c = new Button();
        }

        /// <summary>
        /// メソッド
        /// </summary>
        /// <param name="c">コントロール</param>
        public int CheckOverload(Control c)
        {
            return 10;
        }

        /// <summary>
        /// メソッド
        /// </summary>
        /// <param name="f">フォーム</param>
        public void CheckOverload(ref Form f)
        {
            f = new Form();
        }
    }
}
