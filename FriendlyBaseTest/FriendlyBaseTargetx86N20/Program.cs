using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FriendlyBaseTargetNet20;

namespace FriendlyBaseTargetx86N20
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TargetForm());
        }
    }
}
