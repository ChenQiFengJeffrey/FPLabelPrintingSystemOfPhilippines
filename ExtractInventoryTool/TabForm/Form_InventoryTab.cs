using ExtractInventoryTool.ColumnNameConfig;
using ExtractInventoryTool.EditorForm;
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

namespace ExtractInventoryTool.TabForm
{
    public partial class Form_InventoryTab : Form
    {
        #region 私有属性
        private DataTable _inventoryTable = null;
        #endregion
        public Form_InventoryTab()
        {
            InitializeComponent();
            InitInventoryTable(dataGridView1, new int[] { 0, 1 });//BOM配置
            InitPagerControl();//分页控件
        }

        /// <summary>
        /// 初始化分页控件，每页记录数默认60
        /// </summary>
        public void InitPagerControl()
        {
            pagerControl1.PageIndex = 1;//当前页数
            pagerControl1.PageSize = 60;//每页记录数
            pagerControl1.DrawControl(0);
            pagerControl1.OnPageChanged += PagerControl1_OnPageChanged;
        }
        private void PagerControl1_OnPageChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        /// <summary>
        /// 初始化库存grid
        /// </summary>
        public void InitInventoryTable(DataGridView dv, int[] idAry)
        {
            InventoryConfig client = new InventoryConfig();
            _inventoryTable = new DataTable();
            _inventoryTable.Columns.Add(client.Oid, typeof(string));//0
            _inventoryTable.Columns.Add(client.Material, typeof(Int32));//1
            _inventoryTable.Columns.Add(client.MaterialCode, typeof(string));//2
            _inventoryTable.Columns.Add(client.MaterialName, typeof(string));//3
            _inventoryTable.Columns.Add(client.SupplierCode, typeof(string));//4
            _inventoryTable.Columns.Add(client.SupplierName, typeof(string));//5
            _inventoryTable.Columns.Add(client.SysInventory, typeof(Int32));//6
            _inventoryTable.Columns.Add(client.Min, typeof(Int32));//7
            _inventoryTable.Columns.Add(client.Max, typeof(Int32));//8
            _inventoryTable.Columns.Add(client.HUB, typeof(Int32));//9
            _inventoryTable.Columns.Add(client.InTransit, typeof(Int32));//10
            _inventoryTable.Columns.Add(client.Total, typeof(Int32));//10
            BindGrid(dv, _inventoryTable, idAry);
        }
        /// <summary>
        /// 绑定数据源到GridView
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="dt"></param>
        /// <param name="idAry">需要隐藏的列，从0开始</param>
        public void BindGrid(DataGridView dv, DataTable dt, int[] idAry = null)
        {
            dv.DataSource = dt;
            if (idAry != null)
            {
                foreach (int i in idAry)
                {
                    dv.Columns[i].Visible = false;
                }
            }
            dv.ClearSelection();
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            Task.Run(() => QueryInventory(limit, offset));
            return;
        }
        /// <summary>
        /// 删除库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("请选中需要删除的记录");
                return;
            }
            DialogResult isDelete = MessageBox.Show("确认删除？", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (isDelete != DialogResult.Yes)
                return;
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in rowCollection)
            {
                ids.Add(row.Cells[0].Value.ToString());
            }
            Task.Run(() => DeleteInventoryRecords("Inventory", ids));
        }

        /// <summary>
        /// 清空表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click_Clear(object sender, EventArgs e)
        {
            DialogResult isClear = MessageBox.Show("确认清空所有库存记录？", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (isClear != DialogResult.Yes)
                return;
            Task.Run(() => ClearTable("Inventory"));
        }
        /// <summary>
        /// 导入库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Form_InventoryUploader editor = new Form_InventoryUploader();
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                string limit = pagerControl1.PageSize.ToString();
                string offset = (pagerControl1.PageIndex - 1).ToString();
                Task.Run(() => QueryInventory(limit, offset));
            }
            return;
        }

        #region 查询库存
        /// <summary>
        /// 查询客户回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryInventoryCallbackDel(DataTable dt, int totalCount);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryInventoryCallback(DataTable dt, int totalCount)
        {
            _inventoryTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _inventoryTable.Rows.Add(
                    row["Oid"],
                    row["Material"],
                    row["MaterialCode"],
                    row["MaterialName"],
                    row["SupplierCode"],
                    row["SupplierName"],
                    row["SysInventory"],
                    row["Min"],
                    row["Max"],
                    row["HUB"],
                    row["InTransit"],
                    row["Total"]
                    );
            }
            BindGrid(dataGridView1, _inventoryTable, new int[] { 0, 1 });
            pagerControl1.DrawControl(totalCount);
            return;
        }
        /// <summary>
        /// 查询客户
        /// </summary>
        public void QueryInventory(string limit, string offset)
        {
            int totalCount = 0;
            DataTable dt = new ExtractInventoryTool_InventoryBLL().QueryInventory(limit, offset, out totalCount);
            QueryInventoryCallbackDel del = QueryInventoryCallback;
            dataGridView1.BeginInvoke(del, dt, totalCount);
            return;
        }
        #endregion

        #region 删除库存
        delegate void DeleteRecordsCallbackDel(string errorMessage);
        public void DeleteRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            Task.Run(() => QueryInventory(limit, offset));
        }
        public void DeleteInventoryRecords(string tableName, List<string> ids)
        {
            string errorMessage = string.Empty;
            int result = new ExtractInventoryTool_InventoryBLL().DeleteRecords(tableName, ids, out errorMessage);
            DeleteRecordsCallbackDel del = DeleteRecordsCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }
        #endregion

        #region 清空库存
        delegate void ClearTableCallbackDel(string errorMessage);
        public void ClearTableCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            Task.Run(() => QueryInventory(limit, offset));
        }
        public void ClearTable(string tableName)
        {
            string errorMessage = string.Empty;
            new ExtractInventoryTool_InventoryBLL().ClearTable(tableName, out errorMessage);
            ClearTableCallbackDel del = ClearTableCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }
        #endregion
    }
}
