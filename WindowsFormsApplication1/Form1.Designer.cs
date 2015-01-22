namespace WindowsFormsApplication1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axOPOSPOSPrinter1 = new AxOposPOSPrinter_CCO.AxOPOSPOSPrinter();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axOPOSPOSPrinter1)).BeginInit();
            this.SuspendLayout();
            // 
            // axOPOSPOSPrinter1
            // 
            this.axOPOSPOSPrinter1.Enabled = true;
            this.axOPOSPOSPrinter1.Location = new System.Drawing.Point(22, 26);
            this.axOPOSPOSPrinter1.Name = "axOPOSPOSPrinter1";
            this.axOPOSPOSPrinter1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axOPOSPOSPrinter1.OcxState")));
            this.axOPOSPOSPrinter1.Size = new System.Drawing.Size(192, 102);
            this.axOPOSPOSPrinter1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 213);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.axOPOSPOSPrinter1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axOPOSPOSPrinter1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxOposPOSPrinter_CCO.AxOPOSPOSPrinter axOPOSPOSPrinter1;
        private System.Windows.Forms.Button button1;
    }
}

