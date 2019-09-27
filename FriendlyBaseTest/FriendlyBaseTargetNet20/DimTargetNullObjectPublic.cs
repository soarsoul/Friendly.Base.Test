namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// コンストラクタの引数でnull指定で生成するためのクラス
    /// </summary>
    public class DimTargetNullObjectPublic
    {
        int _intValue;

        /// <summary>
        /// int値
        /// </summary>
        internal int IntValue { get { return _intValue; } set { _intValue = value; } }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="o">オブジェクト</param>
        DimTargetNullObjectPublic(object o)
        {
            _intValue = 100;
        }
    }
}
