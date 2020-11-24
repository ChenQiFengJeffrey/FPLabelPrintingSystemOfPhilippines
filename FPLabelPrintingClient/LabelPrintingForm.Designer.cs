namespace FPLabelPrintingClient
{
    partial class LabelPrintingForm
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelPrintingForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.FinishedProductNum = new System.Windows.Forms.TextBox();
            this.ONU = new System.Windows.Forms.TextBox();
            this.VVDSL = new System.Windows.Forms.TextBox();
            this.TELSET = new System.Windows.Forms.TextBox();
            this.BIZBOX = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.labelID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(62, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 80);
            this.button1.TabIndex = 0;
            this.button1.Text = "START";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(301, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 80);
            this.button2.TabIndex = 1;
            this.button2.Text = "PRINT";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(539, 31);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(130, 80);
            this.button3.TabIndex = 2;
            this.button3.Text = "CLEAR";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(289, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "FinishedProductNum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(379, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "ONU";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(373, 244);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "VVDSL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(361, 277);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "TELSET";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(361, 313);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "BIZBOX";
            // 
            // FinishedProductNum
            // 
            this.FinishedProductNum.Location = new System.Drawing.Point(408, 161);
            this.FinishedProductNum.Name = "FinishedProductNum";
            this.FinishedProductNum.ReadOnly = true;
            this.FinishedProductNum.Size = new System.Drawing.Size(261, 21);
            this.FinishedProductNum.TabIndex = 8;
            this.FinishedProductNum.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FinishedProductNum_KeyUp);
            // 
            // ONU
            // 
            this.ONU.Location = new System.Drawing.Point(408, 203);
            this.ONU.Name = "ONU";
            this.ONU.ReadOnly = true;
            this.ONU.Size = new System.Drawing.Size(261, 21);
            this.ONU.TabIndex = 9;
            this.ONU.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ONU_KeyUp);
            // 
            // VVDSL
            // 
            this.VVDSL.Location = new System.Drawing.Point(408, 240);
            this.VVDSL.Name = "VVDSL";
            this.VVDSL.ReadOnly = true;
            this.VVDSL.Size = new System.Drawing.Size(261, 21);
            this.VVDSL.TabIndex = 10;
            this.VVDSL.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VVDSL_KeyUp);
            // 
            // TELSET
            // 
            this.TELSET.Location = new System.Drawing.Point(408, 274);
            this.TELSET.Name = "TELSET";
            this.TELSET.ReadOnly = true;
            this.TELSET.Size = new System.Drawing.Size(261, 21);
            this.TELSET.TabIndex = 11;
            this.TELSET.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TELSET_KeyUp);
            // 
            // BIZBOX
            // 
            this.BIZBOX.Location = new System.Drawing.Point(408, 310);
            this.BIZBOX.Name = "BIZBOX";
            this.BIZBOX.ReadOnly = true;
            this.BIZBOX.Size = new System.Drawing.Size(261, 21);
            this.BIZBOX.TabIndex = 12;
            this.BIZBOX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BIZBOX_KeyUp);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Red;
            this.button4.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.button4.Location = new System.Drawing.Point(62, 175);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(150, 150);
            this.button4.TabIndex = 13;
            this.button4.UseVisualStyleBackColor = false;
            // 
            // labelID
            // 
            this.labelID.Location = new System.Drawing.Point(408, 347);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(261, 21);
            this.labelID.TabIndex = 14;
            this.labelID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.labelID_KeyUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(379, 350);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "ID";
            // 
            // LabelPrintingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(760, 430);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelID);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.BIZBOX);
            this.Controls.Add(this.TELSET);
            this.Controls.Add(this.VVDSL);
            this.Controls.Add(this.ONU);
            this.Controls.Add(this.FinishedProductNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LabelPrintingForm";
            this.Text = "LabelPrintingSystem";
            this.Load += new System.EventHandler(this.LabelPrintingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox FinishedProductNum;
        private System.Windows.Forms.TextBox ONU;
        private System.Windows.Forms.TextBox VVDSL;
        private System.Windows.Forms.TextBox TELSET;
        private System.Windows.Forms.TextBox BIZBOX;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox labelID;
        private System.Windows.Forms.Label label6;
    }
}

