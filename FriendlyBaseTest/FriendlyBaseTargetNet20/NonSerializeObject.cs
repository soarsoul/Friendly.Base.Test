namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// シリアライズ不可オブジェクト
    /// </summary>
    public class NonSerializeObject
    {
        int _intValue;

        /// <summary>
        /// int値
        /// </summary>
        public int IntValue { get { return _intValue; } set { _intValue = value; } }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NonSerializeObject() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="intValue">int値</param>
        public NonSerializeObject(int intValue)
        {
            _intValue = intValue; 
        }
    }
}
