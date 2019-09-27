namespace FriendlyBaseTargetNet20
{
    partial class TargetForm
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
            this._buttonOtherThread = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.doubleClickAsyncPanel = new FriendlyBaseTargetNet20.PanelEx();
            this.doubleClickPanel = new FriendlyBaseTargetNet20.PanelEx();
            this.clickAsyncPanel = new FriendlyBaseTargetNet20.PanelEx();
            this.clickPanel = new FriendlyBaseTargetNet20.PanelEx();
            this._testUserControl = new FriendlyBaseTargetNet20.TestUserControl();
            this.button1 = new System.Windows.Forms.Button();
            this._panelA = new System.Windows.Forms.Panel();
            this._panelB = new System.Windows.Forms.Panel();
            this._splitContainerC = new System.Windows.Forms.SplitContainer();
            this._checkBoxX = new System.Windows.Forms.CheckBox();
            this._checkBoxY = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this._panelA.SuspendLayout();
            this._panelB.SuspendLayout();
            this._splitContainerC.Panel2.SuspendLayout();
            this._splitContainerC.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonOtherThread
            // 
            this._buttonOtherThread.Location = new System.Drawing.Point(12, 12);
            this._buttonOtherThread.Name = "_buttonOtherThread";
            this._buttonOtherThread.Size = new System.Drawing.Size(179, 23);
            this._buttonOtherThread.TabIndex = 0;
            this._buttonOtherThread.Text = "別スレッドWindow起動";
            this._buttonOtherThread.UseVisualStyleBackColor = true;
            this._buttonOtherThread.Click += new System.EventHandler(this.ButtonOtherThreadClick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 18);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 16);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 40);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(80, 16);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Location = new System.Drawing.Point(217, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 73);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "タイプとウィンドウクラスが同一";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "yyy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "yyy";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(217, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 73);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "WindowTextが同じ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(2, 251);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 50);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(2, 251);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(98, 50);
            this.panel1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "↓ここに矩形の重なったオブジェクトがある";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(177, 253);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 9;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // doubleClickAsyncPanel
            // 
            this.doubleClickAsyncPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.doubleClickAsyncPanel.Location = new System.Drawing.Point(104, 485);
            this.doubleClickAsyncPanel.Name = "doubleClickAsyncPanel";
            this.doubleClickAsyncPanel.Size = new System.Drawing.Size(58, 30);
            this.doubleClickAsyncPanel.TabIndex = 11;
            this.doubleClickAsyncPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.doubleClickAsyncPanel_MouseDoubleClick);
            // 
            // doubleClickPanel
            // 
            this.doubleClickPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.doubleClickPanel.Location = new System.Drawing.Point(104, 449);
            this.doubleClickPanel.Name = "doubleClickPanel";
            this.doubleClickPanel.Size = new System.Drawing.Size(58, 30);
            this.doubleClickPanel.TabIndex = 11;
            this.doubleClickPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.doubleClickPanel_MouseDoubleClick);
            // 
            // clickAsyncPanel
            // 
            this.clickAsyncPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clickAsyncPanel.Location = new System.Drawing.Point(14, 485);
            this.clickAsyncPanel.Name = "clickAsyncPanel";
            this.clickAsyncPanel.Size = new System.Drawing.Size(58, 30);
            this.clickAsyncPanel.TabIndex = 11;
            this.clickAsyncPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.clickAsyncPanel_MouseClick);
            // 
            // clickPanel
            // 
            this.clickPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clickPanel.Location = new System.Drawing.Point(14, 449);
            this.clickPanel.Name = "clickPanel";
            this.clickPanel.Size = new System.Drawing.Size(58, 30);
            this.clickPanel.TabIndex = 10;
            this.clickPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.clickPanel_MouseClick);
            // 
            // _testUserControl
            // 
            this._testUserControl.Location = new System.Drawing.Point(12, 62);
            this._testUserControl.Name = "_testUserControl";
            this._testUserControl.Size = new System.Drawing.Size(150, 150);
            this._testUserControl.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(475, 580);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _panelA
            // 
            this._panelA.Controls.Add(this._panelB);
            this._panelA.Location = new System.Drawing.Point(319, 474);
            this._panelA.Name = "_panelA";
            this._panelA.Size = new System.Drawing.Size(162, 140);
            this._panelA.TabIndex = 13;
            // 
            // _panelB
            // 
            this._panelB.Controls.Add(this._checkBoxX);
            this._panelB.Controls.Add(this._splitContainerC);
            this._panelB.Location = new System.Drawing.Point(17, 11);
            this._panelB.Name = "_panelB";
            this._panelB.Size = new System.Drawing.Size(130, 118);
            this._panelB.TabIndex = 0;
            // 
            // _splitContainerC
            // 
            this._splitContainerC.Location = new System.Drawing.Point(12, 37);
            this._splitContainerC.Name = "_splitContainerC";
            // 
            // _splitContainerC.Panel2
            // 
            this._splitContainerC.Panel2.Controls.Add(this._checkBoxY);
            this._splitContainerC.Size = new System.Drawing.Size(102, 62);
            this._splitContainerC.SplitterDistance = 34;
            this._splitContainerC.TabIndex = 1;
            // 
            // _checkBoxX
            // 
            this._checkBoxX.AutoSize = true;
            this._checkBoxX.Location = new System.Drawing.Point(22, 4);
            this._checkBoxX.Name = "_checkBoxX";
            this._checkBoxX.Size = new System.Drawing.Size(80, 16);
            this._checkBoxX.TabIndex = 2;
            this._checkBoxX.Text = "checkBox3";
            this._checkBoxX.UseVisualStyleBackColor = true;
            // 
            // _checkBoxY
            // 
            this._checkBoxY.AutoSize = true;
            this._checkBoxY.Location = new System.Drawing.Point(4, 13);
            this._checkBoxY.Name = "_checkBoxY";
            this._checkBoxY.Size = new System.Drawing.Size(80, 16);
            this._checkBoxY.TabIndex = 0;
            this._checkBoxY.Text = "checkBox3";
            this._checkBoxY.UseVisualStyleBackColor = true;
            // 
            // TargetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 626);
            this.Controls.Add(this._panelA);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.doubleClickAsyncPanel);
            this.Controls.Add(this.doubleClickPanel);
            this.Controls.Add(this.clickAsyncPanel);
            this.Controls.Add(this.clickPanel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._testUserControl);
            this.Controls.Add(this._buttonOtherThread);
            this.Name = "TargetForm";
            this.Text = "TargetForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this._panelA.ResumeLayout(false);
            this._panelB.ResumeLayout(false);
            this._panelB.PerformLayout();
            this._splitContainerC.Panel2.ResumeLayout(false);
            this._splitContainerC.Panel2.PerformLayout();
            this._splitContainerC.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonOtherThread;
        private TestUserControl _testUserControl;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private PanelEx clickPanel;
        private PanelEx clickAsyncPanel;
        private PanelEx doubleClickPanel;
        private PanelEx doubleClickAsyncPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel _panelA;
        private System.Windows.Forms.Panel _panelB;
        private System.Windows.Forms.CheckBox _checkBoxX;
        private System.Windows.Forms.SplitContainer _splitContainerC;
        private System.Windows.Forms.CheckBox _checkBoxY;
    }
}