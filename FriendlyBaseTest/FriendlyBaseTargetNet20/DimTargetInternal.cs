namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// internalなDim宣言テスト用クラス
    /// </summary>
    class DimTargetInternal
    {
        int _intValue;

        /// <summary>
        /// int値の取得、設定
        /// </summary>
        internal int IntValue { get { return _intValue; } set { _intValue = value; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        DimTargetInternal() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="intValue">int値</param>
        DimTargetInternal(int intValue)
        {
            _intValue = intValue;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="intValue">long値</param>
        protected DimTargetInternal(long longValue)
        {
            _intValue = (int)longValue * 10;
        }
    }
}
