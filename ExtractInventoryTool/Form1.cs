using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using ExtractInventoryTool.ColumnNameConfig;
using ExtractInventoryTool.EditorForm;
using ExtractInventoryTool.TabForm;
using ExtractInventoryTool.ToolForm;
using FPLabelData.Entity;
using LabelPrintDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractInventoryTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LogHelper.Setup();//启动Log4net
        }


        /// <summary>
        /// 绑定数据源到GridView
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="dt"></param>
        /// <param name="idAry">需要隐藏的列，从0开始</param>
        void bindGrid(DataGridView dv, DataTable dt, int[] idAry = null)
        {
            dv.DataSource = dt;
            if (idAry != null)
            {
                foreach (int i in idAry)
                {
                    dv.Columns[i].Visible = false;
                }
            }
            //dv.AutoResizeColumns();
            dv.ClearSelection();
        }

        /// <summary>
        /// 左侧导航栏链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navBarControl1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            DevExpress.XtraNavBar.ICollectionItem item = e.Link as DevExpress.XtraNavBar.ICollectionItem;
            string tabName = item.ItemName.Trim().ToLower();
            //先判断tab项是否已打开
            // Meritar_Jeffrey	2021/03/28 17:54:01
            foreach (TabPage page in tabControl1.TabPages)
            {
                if (page.Name == tabName)
                {
                    tabControl1.SelectedTab = page;//显示该页
                    return;
                }
            }
            TabPage tab = new TabPage();
            switch (tabName)
            {
                case "client"://客户
                    tab.Name = "client";
                    tab.Text = "客户";
                    Form_ClientTab clientTab = new Form_ClientTab();
                    clientTab.TopLevel = false;
                    clientTab.Dock = DockStyle.Fill;
                    clientTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(clientTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    clientTab.Show();
                    break;
                case "material"://物料备案
                    tab.Name = "material";
                    tab.Text = "物料备案";
                    Form_MaterialTab materialTab = new Form_MaterialTab();
                    materialTab.TopLevel = false;
                    materialTab.Dock = DockStyle.Fill;
                    materialTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(materialTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    materialTab.Show();
                    break;
                case "bom"://bom
                    tab.Name = "bom";
                    tab.Text = "BOM";
                    Form_BOMTab bomTab = new Form_BOMTab();
                    bomTab.TopLevel = false;
                    bomTab.Dock = DockStyle.Fill;
                    bomTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(bomTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    bomTab.Show();
                    break;
                case "inventory"://bom
                    tab.Name = "inventory";
                    tab.Text = "库存";
                    Form_InventoryTab inventoryTab = new Form_InventoryTab();
                    inventoryTab.TopLevel = false;
                    inventoryTab.Dock = DockStyle.Fill;
                    inventoryTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(inventoryTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    inventoryTab.Show();
                    break;
                case "productionplan"://bom
                    tab.Name = "productionplan";
                    tab.Text = "生产计划";
                    Form_ProductionPlanTab planTab = new Form_ProductionPlanTab();
                    planTab.TopLevel = false;
                    planTab.Dock = DockStyle.Fill;
                    planTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(planTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    planTab.Show();
                    break;
                case "pickupdemand"://bom
                    tab.Name = "pickupdemand";
                    tab.Text = "取货需求";
                    Form_PickUpDemandTab pickupTab = new Form_PickUpDemandTab();
                    pickupTab.TopLevel = false;
                    pickupTab.Dock = DockStyle.Fill;
                    pickupTab.FormBorderStyle = FormBorderStyle.None;
                    tab.Controls.Add(pickupTab);
                    tabControl1.TabPages.Add(tab);
                    tabControl1.SelectedTab = tab;
                    pickupTab.Show();
                    break;
                default:
                    break;
            }
            return;
        }

        /// <summary>
        /// 关闭tabcontrol选项卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabCount == 0)
            {
                return;
            }
            this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);
        }

        /// <summary>
        /// 关闭所有选项卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = tabControl1.TabCount - 1; i >= 0; i--)
            {
                this.tabControl1.TabPages.RemoveAt(i);
            }
        }
    }
}
