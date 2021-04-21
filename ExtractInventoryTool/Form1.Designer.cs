namespace ExtractInventoryTool
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.Client = new DevExpress.XtraNavBar.NavBarItem();
            this.material = new DevExpress.XtraNavBar.NavBarItem();
            this.bom = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarGroup2 = new DevExpress.XtraNavBar.NavBarGroup();
            this.productionplan = new DevExpress.XtraNavBar.NavBarItem();
            this.pickupdemand = new DevExpress.XtraNavBar.NavBarItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.inventory = new DevExpress.XtraNavBar.NavBarItem();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroup1;
            this.navBarControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroup1,
            this.navBarGroup2});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.Client,
            this.material,
            this.bom,
            this.productionplan,
            this.pickupdemand,
            this.inventory});
            this.navBarControl1.Location = new System.Drawing.Point(1, 5);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 146;
            this.navBarControl1.Size = new System.Drawing.Size(146, 405);
            this.navBarControl1.TabIndex = 0;
            this.navBarControl1.Text = "navBarControl1";
            this.navBarControl1.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarControl1_LinkClicked);
            // 
            // navBarGroup1
            // 
            this.navBarGroup1.Caption = "基础数据";
            this.navBarGroup1.Expanded = true;
            this.navBarGroup1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.Client),
            new DevExpress.XtraNavBar.NavBarItemLink(this.material),
            new DevExpress.XtraNavBar.NavBarItemLink(this.bom),
            new DevExpress.XtraNavBar.NavBarItemLink(this.inventory)});
            this.navBarGroup1.Name = "navBarGroup1";
            // 
            // Client
            // 
            this.Client.Caption = "客户";
            this.Client.Name = "Client";
            // 
            // material
            // 
            this.material.Caption = "物料备案";
            this.material.Name = "material";
            // 
            // bom
            // 
            this.bom.Caption = "BOM";
            this.bom.Name = "bom";
            // 
            // navBarGroup2
            // 
            this.navBarGroup2.Caption = "数据转换";
            this.navBarGroup2.Expanded = true;
            this.navBarGroup2.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.productionplan),
            new DevExpress.XtraNavBar.NavBarItemLink(this.pickupdemand)});
            this.navBarGroup2.Name = "navBarGroup2";
            // 
            // productionplan
            // 
            this.productionplan.Caption = "生产计划";
            this.productionplan.Name = "productionplan";
            // 
            // pickupdemand
            // 
            this.pickupdemand.Caption = "取货需求";
            this.pickupdemand.Name = "pickupdemand";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Location = new System.Drawing.Point(153, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(747, 376);
            this.tabControl1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(729, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "关闭选项卡";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(819, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "关闭所有";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // inventory
            // 
            this.inventory.Caption = "库存";
            this.inventory.Name = "inventory";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 412);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.navBarControl1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "取货需求转换工具";
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1;
        private DevExpress.XtraNavBar.NavBarItem Client;
        private DevExpress.XtraNavBar.NavBarItem material;
        private DevExpress.XtraNavBar.NavBarItem bom;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup2;
        private DevExpress.XtraNavBar.NavBarItem productionplan;
        private DevExpress.XtraNavBar.NavBarItem pickupdemand;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private DevExpress.XtraNavBar.NavBarItem inventory;
    }
}

