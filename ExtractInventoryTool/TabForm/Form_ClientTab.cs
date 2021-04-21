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
    public partial class Form_ClientTab : Form
    {
        #region 私有属性
        private DataTable _clientTable = null;
        #endregion
        public Form_ClientTab()
        {
            InitializeComponent();
            InitClientTable(dataGridView1, new int[] { 0 });//客户配置
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
        public void InitClientTable(DataGridView dv, int[] idAry)
        {
            ClientConfig client = new ClientConfig();
            _clientTable = new DataTable();
            _clientTable.Columns.Add(client.Oid, typeof(string));
            _clientTable.Columns.Add(client.Name, typeof(string));
            _clientTable.Columns.Add(client.UniqueCode, typeof(string));
            _clientTable.Columns.Add(client.Remark, typeof(string));
            _clientTable.Columns.Add(client.RegexRule, typeof(string));
            BindGrid(dv, _clientTable, idAry);
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
        /// 查询客户事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string limit = pagerControl1.PageSize.ToString(); 
            string offset = (pagerControl1.PageIndex - 1).ToString();
            Task.Run(() => QueryClient(limit, offset));
            return;
        }

        #region 查询客户
        /// <summary>
        /// 查询客户回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryClientCallbackDel(DataTable dt, int totalCount);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryClientCallback(DataTable dt,int totalCount)
        {
            _clientTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _clientTable.Rows.Add(
                    row["Oid"],
                    row["Name"],
                    row["UniqueCode"],
                    row["Remark"],
                    row["RegexRule"]
                    );
            }
            BindGrid(dataGridView1, _clientTable, new int[] { 0 });
            pagerControl1.DrawControl(totalCount);
            return;
        }
        /// <summary>
        /// 查询客户
        /// </summary>
        public void QueryClient(string limit, string offset)
        {
            int totalCount = 0;
            DataTable dt = new ExtractInventoryTool_ClientBLL().QueryClient(limit, offset, out totalCount);
            QueryClientCallbackDel del = QueryClientCallback;
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
            Form_ClientEditor editor = new Form_ClientEditor(null);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                string limit = pagerControl1.PageSize.ToString();
                string offset = (pagerControl1.PageIndex - 1).ToString();
                Task.Run(() => QueryClient(limit, offset));
            }
            return;
        }

        /// <summary>
        /// 更新客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ExtractInventoryTool_Client client = null;
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
                client = new ExtractInventoryTool_Client();
                int oid = 0;
                client.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                client.Name = row.Cells[1].Value.ToString().Trim();
                client.UniqueCode = row.Cells[2].Value.ToString().Trim();
                client.Remark = row.Cells[3].Value.ToString().Trim();
                client.RegexRule = row.Cells[4].Value.ToString().Trim();
            }
            Form_ClientEditor editor = new Form_ClientEditor(client);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                string limit = pagerControl1.PageSize.ToString();
                string offset = (pagerControl1.PageIndex - 1).ToString();
                Task.Run(() => QueryClient(limit, offset));
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
            Task.Run(() => DeleteClientRecords("Client", ids));
        }

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
            Task.Run(() => QueryClient(limit, offset));
        }
        public void DeleteClientRecords(string tableName,List<string> ids) {
            string errorMessage = string.Empty;
            int result = new ExtractInventoryTool_ClientBLL().DeleteRecords(tableName, ids,out errorMessage);
            DeleteRecordsCallbackDel del = DeleteRecordsCallback;
            dataGridView1.BeginInvoke(del,errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }
    }
}
