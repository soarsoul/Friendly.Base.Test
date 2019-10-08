using System;
using System.Collections.Generic;
using System.Text;
using FriendlyBaseTargetNet20;
using System.Runtime.InteropServices;

namespace ExpandTestTarget
{
    public static class ExpandMethods
    {
        static void SetText(string text, TargetForm form)
        {
            form.Text = text;
        }

        static int GetDataValue(Data data)
        {
            return data._data;
        }

        [DllImport("ExpandTestTargetNative.dll", CharSet = CharSet.Ansi)]
        internal static extern int Func(string msg);
    }

    [Serializable]
    public class Data
    {
        public int _data;
        public Data(int data)
        {
            _data = data;
        }
    }
}
