namespace FriendlyBaseTargetNet20
{
    partial class NormalGuiForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._textBoxSrc = new System.Windows.Forms.TextBox();
            this._buttonCopyText = new System.Windows.Forms.Button();
            this._textBoxDst = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _textBoxSrc
            // 
            this._textBoxSrc.Location = new System.Drawing.Point(12, 12);
            this._textBoxSrc.Name = "_textBoxSrc";
            this._textBoxSrc.Size = new System.Drawing.Size(100, 19);
            this._textBoxSrc.TabIndex = 0;
            // 
            // _buttonCopyText
            // 
            this._buttonCopyText.Location = new System.Drawing.Point(24, 37);
            this._buttonCopyText.Name = "_buttonCopyText";
            this._buttonCopyText.Size = new System.Drawing.Size(75, 23);
            this._buttonCopyText.TabIndex = 1;
            this._buttonCopyText.Text = "↓";
            this._buttonCopyText.UseVisualStyleBackColor = true;
            this._buttonCopyText.Click += new System.EventHandler(this.ButtonCopyTextClick);
            // 
            // _textBoxDst
            // 
            this._textBoxDst.Location = new System.Drawing.Point(12, 66);
            this._textBoxDst.Name = "_textBoxDst";
            this._textBoxDst.Size = new System.Drawing.Size(100, 19);
            this._textBoxDst.TabIndex = 2;
            // 
            // NormalGuiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(137, 109);
            this.Controls.Add(this._textBoxDst);
            this.Controls.Add(this._buttonCopyText);
            this.Controls.Add(this._textBoxSrc);
            this.Name = "NormalGuiForm";
            this.Text = "NormalGuiForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textBoxSrc;
        private System.Windows.Forms.Button _buttonCopyText;
        private System.Windows.Forms.TextBox _textBoxDst;
    }
}