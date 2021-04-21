namespace CustomWinFormControl
{
    partial class PagerControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPageNum = new System.Windows.Forms.TextBox();
            this.linkLast = new System.Windows.Forms.Button();
            this.linkNext = new System.Windows.Forms.Button();
            this.linkPrevious = new System.Windows.Forms.Button();
            this.linkFirst = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblPageCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCurrentPage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPageSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGo.Location = new System.Drawing.Point(558, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(44, 23);
            this.btnGo.TabIndex = 31;
            this.btnGo.Text = "转到";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPageNum
            // 
            this.txtPageNum.Location = new System.Drawing.Point(513, 4);
            this.txtPageNum.Name = "txtPageNum";
            this.txtPageNum.Size = new System.Drawing.Size(39, 21);
            this.txtPageNum.TabIndex = 30;
            this.txtPageNum.Text = "1";
            this.txtPageNum.TextChanged += new System.EventHandler(this.txtPageNum_TextChanged);
            // 
            // linkLast
            // 
            this.linkLast.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLast.Location = new System.Drawing.Point(477, 3);
            this.linkLast.Name = "linkLast";
            this.linkLast.Size = new System.Drawing.Size(30, 23);
            this.linkLast.TabIndex = 29;
            this.linkLast.Text = ">>";
            this.linkLast.UseVisualStyleBackColor = true;
            this.linkLast.Click += new System.EventHandler(this.linkLast_Click);
            // 
            // linkNext
            // 
            this.linkNext.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkNext.Location = new System.Drawing.Point(441, 3);
            this.linkNext.Name = "linkNext";
            this.linkNext.Size = new System.Drawing.Size(30, 23);
            this.linkNext.TabIndex = 28;
            this.linkNext.Text = ">";
            this.linkNext.UseVisualStyleBackColor = true;
            this.linkNext.Click += new System.EventHandler(this.linkNext_Click);
            // 
            // linkPrevious
            // 
            this.linkPrevious.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkPrevious.Location = new System.Drawing.Point(397, 3);
            this.linkPrevious.Name = "linkPrevious";
            this.linkPrevious.Size = new System.Drawing.Size(30, 23);
            this.linkPrevious.TabIndex = 27;
            this.linkPrevious.Text = "<";
            this.linkPrevious.UseVisualStyleBackColor = true;
            this.linkPrevious.Click += new System.EventHandler(this.linkPrevious_Click);
            // 
            // linkFirst
            // 
            this.linkFirst.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkFirst.Location = new System.Drawing.Point(361, 3);
            this.linkFirst.Name = "linkFirst";
            this.linkFirst.Size = new System.Drawing.Size(30, 23);
            this.linkFirst.TabIndex = 26;
            this.linkFirst.Text = "<<";
            this.linkFirst.UseVisualStyleBackColor = true;
            this.linkFirst.Click += new System.EventHandler(this.linkFirst_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(308, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 25;
            this.label9.Text = "条记录";
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalCount.Location = new System.Drawing.Point(258, 8);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(12, 12);
            this.lblTotalCount.TabIndex = 24;
            this.lblTotalCount.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(240, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 23;
            this.label7.Text = "共";
            // 
            // lblPageCount
            // 
            this.lblPageCount.AutoSize = true;
            this.lblPageCount.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPageCount.Location = new System.Drawing.Point(210, 8);
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(12, 12);
            this.lblPageCount.TabIndex = 22;
            this.lblPageCount.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(193, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "/";
            // 
            // lblCurrentPage
            // 
            this.lblCurrentPage.AutoSize = true;
            this.lblCurrentPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCurrentPage.Location = new System.Drawing.Point(176, 8);
            this.lblCurrentPage.Name = "lblCurrentPage";
            this.lblCurrentPage.Size = new System.Drawing.Size(12, 12);
            this.lblCurrentPage.TabIndex = 20;
            this.lblCurrentPage.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(117, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "当前页：";
            // 
            // txtPageSize
            // 
            this.txtPageSize.Location = new System.Drawing.Point(45, 5);
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Size = new System.Drawing.Size(39, 21);
            this.txtPageSize.TabIndex = 18;
            this.txtPageSize.Text = "60";
            this.txtPageSize.TextChanged += new System.EventHandler(this.txtPageSize_TextChanged);
            this.txtPageSize.Leave += new System.EventHandler(this.txtPageSize_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "条";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "每页";
            // 
            // PagerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtPageNum);
            this.Controls.Add(this.linkLast);
            this.Controls.Add(this.linkNext);
            this.Controls.Add(this.linkPrevious);
            this.Controls.Add(this.linkFirst);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblTotalCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblPageCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblCurrentPage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPageSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PagerControl";
            this.Size = new System.Drawing.Size(607, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPageNum;
        private System.Windows.Forms.Button linkLast;
        private System.Windows.Forms.Button linkNext;
        private System.Windows.Forms.Button linkPrevious;
        private System.Windows.Forms.Button linkFirst;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblPageCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCurrentPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPageSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
