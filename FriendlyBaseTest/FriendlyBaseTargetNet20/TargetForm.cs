using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// テスト対象メインウィンドウ
    /// </summary>
    public partial class TargetForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TargetForm()
        {
            InitializeComponent();
            if (1 < Environment.GetCommandLineArgs().Length && Environment.GetCommandLineArgs()[1] == "splash")
            {
                Thread.Sleep(1000);
                Form f = new Form();
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                t.Interval = 10;
                t.Tick += delegate
                {
                    if (Process.GetCurrentProcess().MainWindowHandle == f.Handle)
                    {
                        f.Close();
                    }
                };
                t.Start();
                f.ShowDialog();
            }
        }

        public void StartNewDomain()
        {
            AppDomain.CreateDomain("new domain").DoCallBack(() => new Form().Show());
        }

        /// <summary>
        /// 別スレッドでウィンドウを表示する
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント内容</param>
        private void ButtonOtherThreadClick(object sender, EventArgs e)
        {
            Thread s = new Thread(new ThreadStart(delegate
            {
                using (NormalGuiForm f = new NormalGuiForm())
                {
                    f.ShowDialog();
                }
            }));
            s.Start();
        }

        public int _overLoadCheckValue;

        /// <summary>
        /// オーバーロードテスト用
        /// </summary>
        /// <param name="c">コントロール</param>
        public void Func(Control c)
        {
            _overLoadCheckValue = 1;
        }

        /// <summary>
        /// オーバーロードテスト用
        /// </summary>
        /// <param name="f">フォーム</param>
        public void Func(Form f)
        {
            _overLoadCheckValue = 2;
        }

        public int _msg;
        public IntPtr _wparam;
        public IntPtr _lparam;
        public int _counter;
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        int _incValue;

        void Inc()
        {
            _incValue++;
        }
        /// <summary>
        /// SendMessage系のテスト用
        /// </summary>
        /// <param name="m">メッセージ情報</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x8001)
            {
                _wparam = m.WParam;
                _lparam = m.LParam;
            }
            else if (m.Msg == 0x8002)
            {
                _wparam = m.WParam;
                _lparam = m.LParam;
                MessageBox.Show("");
            }
            else if (m.Msg == 0x8003)
            {
                Thread.Sleep(5000);
            }
            else if (m.Msg == 0x8004)
            {
                _counter++;
            }
            else if (m.Msg == 0x8005)
            {
                for (int i = 0; i < 200000; i++)
                {
                    PostMessage(Handle, 0x8006, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(3000);
            }
            base.WndProc(ref m);
        }

        int AsyncValueTest(out int arg)
        {
            MessageBox.Show("");
            arg = 1;
            return 2;
        }

        public int _mouseCheckValue;
        public Point _mousePos;
        public MouseButtons _mouseButton;

        private void clickPanel_MouseClick(object sender, MouseEventArgs e)
        {
            _mouseButton = e.Button;
            _mousePos = e.Location;
            _mouseCheckValue = 1;
        }

        private void clickAsyncPanel_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("");
            _mouseButton = e.Button;
            _mousePos = e.Location;
            _mouseCheckValue = 2;
        }

        private void doubleClickPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _mouseButton = e.Button;
            _mousePos = e.Location;
            _mouseCheckValue = 3;
        }

        private void doubleClickAsyncPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("");
            _mouseButton = e.Button;
            _mousePos = e.Location;
            _mouseCheckValue = 4;
        }

        int _clickCount = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            _clickCount++;
        }

        public bool Proccess { get; set; }
    }

    class PanelEx : Panel
    {
        public int _msg;
        public IntPtr _wparam;
        public IntPtr _lparam;

        /// <summary>
        /// SendMessage系のテスト用
        /// </summary>
        /// <param name="m">メッセージ情報</param>
        protected override void WndProc(ref Message m)
        {
            if (0x0201 <= m.Msg && m.Msg <= 0x020E)
            {
                _msg = m.Msg;
                _wparam = m.WParam;
                _lparam = m.LParam;
            }
            base.WndProc(ref m);
        }
    }
}
