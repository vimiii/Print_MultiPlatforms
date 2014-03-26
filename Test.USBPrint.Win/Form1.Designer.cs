namespace Test.USBPrint.Win
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnInite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVid = new System.Windows.Forms.TextBox();
            this.txtPid = new System.Windows.Forms.TextBox();
            this.btnString = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(110, 165);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "打印图片";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnInite
            // 
            this.btnInite.Location = new System.Drawing.Point(21, 165);
            this.btnInite.Name = "btnInite";
            this.btnInite.Size = new System.Drawing.Size(75, 23);
            this.btnInite.TabIndex = 1;
            this.btnInite.Text = "开启usb连接";
            this.btnInite.UseVisualStyleBackColor = true;
            this.btnInite.Click += new System.EventHandler(this.btnInite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "VID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "PID";
            // 
            // txtVid
            // 
            this.txtVid.Location = new System.Drawing.Point(110, 26);
            this.txtVid.Name = "txtVid";
            this.txtVid.Size = new System.Drawing.Size(100, 21);
            this.txtVid.TabIndex = 4;
            this.txtVid.Text = "4070";
            // 
            // txtPid
            // 
            this.txtPid.Location = new System.Drawing.Point(110, 64);
            this.txtPid.Name = "txtPid";
            this.txtPid.Size = new System.Drawing.Size(100, 21);
            this.txtPid.TabIndex = 5;
            this.txtPid.Text = "33054";
            // 
            // btnString
            // 
            this.btnString.Location = new System.Drawing.Point(191, 165);
            this.btnString.Name = "btnString";
            this.btnString.Size = new System.Drawing.Size(75, 23);
            this.btnString.TabIndex = 6;
            this.btnString.Text = "打印文字";
            this.btnString.UseVisualStyleBackColor = true;
            this.btnString.Click += new System.EventHandler(this.btnString_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnString);
            this.Controls.Add(this.txtPid);
            this.Controls.Add(this.txtVid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInite);
            this.Controls.Add(this.btnPrint);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnInite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVid;
        private System.Windows.Forms.TextBox txtPid;
        private System.Windows.Forms.Button btnString;
    }
}

