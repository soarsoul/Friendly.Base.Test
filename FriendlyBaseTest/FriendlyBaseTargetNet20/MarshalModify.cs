using System.Runtime.InteropServices;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// In,Out修飾があっても正常に呼び出しができることの確認
    /// </summary>
    public static class MarshalModify
    {
        public static int TestIn([In]int val)
        {
            return val;
        }
        public static int TestOut([Out]int val)
        {
            return val;
        }
        public static int TestInOut([In, Out]int val)
        {
            return val;
        }

        public static int TestInRef([In]ref int val)
        {
            return val;
        }

        public static int TestInOutRef([In, Out]ref int val)
        {
            return val;
        }

        public static int TestOutOut([Out]out int val)
        {
            val = 100;
            return val;
        }
    }
}
