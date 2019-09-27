using System;
using System.Windows.Forms;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// 宣言テスト用クラス
    /// </summary>
    [Serializable]
    public class DimTargetPublic
    {
        int _intValue;

        /// <summary>
        /// int値の取得、設定
        /// </summary>
        public int IntValue { get { return _intValue; } set { _intValue = value; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DimTargetPublic() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="intValue">int値</param>
        public DimTargetPublic(int intValue)
        {
            _intValue = intValue; 
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">コントロール</param>
        public DimTargetPublic(Control c)
        {
            _intValue = 10;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="f">フォーム</param>
        public DimTargetPublic(Form f)
        {
            _intValue = 100;
        }
    }
}
