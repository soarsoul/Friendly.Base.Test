using System;
using System.Windows.Forms;

namespace FriendlyBaseTargetNet20
{
    /// <summary>
    /// GUIオブジェクトテスト用
    /// </summary>
    public partial class NormalGuiForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NormalGuiForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント内容</param>
        private void ButtonCopyTextClick(object sender, EventArgs e)
        {
            _textBoxDst.Text = _textBoxSrc.Text;
        }
    }
}
