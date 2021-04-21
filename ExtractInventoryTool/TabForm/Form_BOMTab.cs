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
    public partial class Form_BOMTab : Form
    {
        #region 私有属性
        private DataTable _bomTable = null;
        #endregion
        public Form_BOMTab()
        {
            InitializeComponent();
            InitBOMTable(dataGridView1, new int[] { 0, 2, 3, 5, 8 });//BOM配置
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
        /// 初始化客户grid
        /// </summary>
        public void InitBOMTable(DataGridView dv, int[] idAry)
        {
            BOMConfig client = new BOMConfig();
            _bomTable = new DataTable();
            _bomTable.Columns.Add(client.Oid, typeof(string));//0
            _bomTable.Columns.Add(client.VehicleModelCode, typeof(string));//1车型代码
            _bomTable.Columns.Add(client.Material, typeof(string));//2
            _bomTable.Columns.Add(client.Material_Name, typeof(string));//3
            _bomTable.Columns.Add(client.Material_Code, typeof(string));//4 物料号
            _bomTable.Columns.Add(client.Material_SupplierName, typeof(string));//5
            _bomTable.Columns.Add(client.Material_SupplierCode, typeof(string));//6 供应商代码
            _bomTable.Columns.Add(client.Client_Name, typeof(string));//7 客户
            _bomTable.Columns.Add(client.Client_Code, typeof(string));//8
            _bomTable.Columns.Add(client.UnitUsage, typeof(int));//9 单车用量
            _bomTable.Columns.Add(client.UpdateTime, typeof(DateTime));//10 修改时间
            BindGrid(dv, _bomTable, idAry);
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
        /// 查询bom事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            QueryBOM();
            return;
        }

        public void QueryBOM()
        {
            #region 分页
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            #endregion
            #region where查询
            StringBuilder whereStrbd = new StringBuilder();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                whereStrbd.Append(string.Format(" and bo.VehicleModelCode like '%{0}%' ", textBox1.Text));
            }
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                whereStrbd.Append(string.Format(" and mt.Code like '%{0}%' ", textBox2.Text));
            }
            #endregion
            Task.Run(() => QueryBOM(limit, offset, whereStrbd.ToString()));
            return;
        }

        #region 查询客户
        /// <summary>
        /// 查询客户回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryBOMCallbackDel(DataTable dt, int totalCount);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryBOMCallback(DataTable dt, int totalCount)
        {
            _bomTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _bomTable.Rows.Add(
                    row["Oid"],
                    row["VehicleModelCode"],
                    row["Material"],
                    row["MaterialName"],
                    row["MaterialCode"],
                    row["SupplierName"],
                    row["SupplierCode"],
                    row["ClientName"],
                    row["ClientCode"],
                    row["UnitUsage"],
                    row["UpdateTime"]
                    );
            }
            BindGrid(dataGridView1, _bomTable, new int[] { 0, 2, 3, 5, 8 });
            pagerControl1.DrawControl(totalCount);
            return;
        }
        /// <summary>
        /// 查询BOM
        /// </summary>
        public void QueryBOM(string limit, string offset, string whereStr)
        {
            int totalCount = 0;
            DataTable dt = new ExtractInventoryTool_BOMBLL().QueryBOM(limit, offset, whereStr, out totalCount);
            QueryBOMCallbackDel del = QueryBOMCallback;
            dataGridView1.BeginInvoke(del, dt, totalCount);
            return;
        }
        #endregion

        /// <summary>
        /// 新增客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form_BOMEditor editor = new Form_BOMEditor(null);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryBOM());
            }
            return;
        }

        /// <summary>
        /// 更新BOM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ExtractInventoryTool_BOM bom = null;
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count != 1)
            {
                MessageBox.Show("只能选中一条记录进行更新", "Warning");
                return;
            }
            row = rowCollection[0];
            if (row != null && row.Cells[0].Value != null)
            {
                bom = new ExtractInventoryTool_BOM();
                int oid = 0;
                bom.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                bom.VehicleModelCode = row.Cells[1].Value.ToString().Trim();
                int materialOid = 0;
                bom.Material = int.TryParse(row.Cells[2].Value.ToString().Trim(), out materialOid) ? materialOid : 0;
                int unitUsage = 0;
                bom.UnitUsage = int.TryParse(row.Cells[9].Value.ToString().Trim(), out unitUsage) ? unitUsage : 0;
                bom.UpdateTime = (DateTime)row.Cells[10].Value;
            }
            Form_BOMEditor editor = new Form_BOMEditor(bom);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryBOM());
            }
            return;
        }

        /// <summary>
        /// 删除客户
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
            Task.Run(() => DeleteBOMRecords("BOM", ids));
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Form_BOMUploader editor = new Form_BOMUploader();
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryBOM());
            }
            return;
        }

        delegate void DeleteRecordsCallbackDel(string errorMessage);
        public void DeleteRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            Task.Run(() => QueryBOM());
        }
        public void DeleteBOMRecords(string tableName, List<string> ids)
        {
            string errorMessage = string.Empty;
            int result = new ExtractInventoryTool_BOMBLL().DeleteRecords(tableName, ids, out errorMessage);
            DeleteRecordsCallbackDel del = DeleteRecordsCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }

        /// <summary>
        /// 清空BOM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult isDelete = MessageBox.Show("此操作会清空所有记录，确认清空？", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (isDelete != DialogResult.Yes)
                return;
            Task.Run(() => ClearPlanRecords("BOM"));
        }

        #region 清空BOM
        delegate void ClearRecordsCallbackDel(string errorMessage);
        public void ClearRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            QueryBOM();
        }
        public void ClearPlanRecords(string tableName)
        {
            string errorMessage = string.Empty;
            new ExtractInventoryTool_ProductionPlanBLL().ClearTable(tableName, out errorMessage);
            ClearRecordsCallbackDel del = ClearRecordsCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }
        #endregion
    }
}
