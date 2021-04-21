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
    public partial class Form_ProductionPlanTab : Form
    {
        #region 私有属性
        private DataTable _planTable = null;
        #endregion

        public Form_ProductionPlanTab()
        {
            InitializeComponent();
            InitPlanTable(dataGridView1, new int[] { 0, 1, 7 });//ProductionPlan配置
            InitPagerControl();//分页控件
            InitDateTimePicker();//时间控件
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

        /// <summary>
        /// 初始化时间控件
        /// </summary>
        public void InitDateTimePicker()
        {
            this.textBox1.Text = "";
            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = " ";
            this.dateTimePicker1.Text = "";
            this.dateTimePicker2.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker2.CustomFormat = " ";
            this.dateTimePicker2.Text = "";
        }

        private void PagerControl1_OnPageChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        #region 初始化生产计划
        /// <summary>
        /// 初始化生产计划grid
        /// </summary>
        public void InitPlanTable(DataGridView dv, int[] idAry)
        {
            PlanConfig plan = new PlanConfig();
            _planTable = new DataTable();
            _planTable.Columns.Add(plan.Oid, typeof(Int32));//0
            _planTable.Columns.Add(plan.Client, typeof(Int32));//1
            _planTable.Columns.Add(plan.ClientCode, typeof(string));//2
            _planTable.Columns.Add(plan.VehicleModelCode, typeof(string));//3
            _planTable.Columns.Add(plan.ppProductionDate, typeof(DateTime));//4
            _planTable.Columns.Add(plan.UnitNum, typeof(Int32));//5
            _planTable.Columns.Add(plan.UpdateTime, typeof(DateTime));//6
            _planTable.Columns.Add(plan.UniqueCode, typeof(string));//7
            BindGrid(dv, _planTable, idAry);
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

        #endregion

        /// <summary>
        /// 查询生产计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            QueryProductionPlan();
            return;
        }

        /// <summary>
        /// 删除生产计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
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
            Task.Run(() => DeletePlanRecords("ProductionPlan", ids));
        }
        /// <summary>
        /// 清空计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult isDelete = MessageBox.Show("此操作会清空所有记录，确认清空？", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (isDelete != DialogResult.Yes)
                return;
            Task.Run(() => ClearPlanRecords("ProductionPlan"));
        }
        /// <summary>
        /// 导入生产计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Form_ProductionPlanUploader editor = new Form_ProductionPlanUploader();
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryProductionPlan());
            }
            return;
        }

        #region 查询生产计划
        /// <summary>
        /// 查询生产计划回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryProductionPlanCallbackDel(DataTable dt, int totalCount);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryProductionPlanCallback(DataTable dt, int totalCount)
        {
            _planTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _planTable.Rows.Add(
                    row["Oid"],
                    row["Client"],
                    row["ClientCode"],
                    row["VehicleModelCode"],
                    row["ppProductionDate"],
                    row["UnitNum"],
                    row["UpdateTime"],
                    row["UniqueCode"]
                    );
            }
            BindGrid(dataGridView1, _planTable, new int[] { 0, 1 });
            pagerControl1.DrawControl(totalCount);
            return;
        }
        /// <summary>
        /// 查询客户
        /// </summary>
        public void QueryProductionPlan(string limit, string offset, string whereStr)
        {
            int totalCount = 0;
            DataTable dt = new ExtractInventoryTool_ProductionPlanBLL().QueryProductionPlan(limit, offset, whereStr, out totalCount);
            QueryProductionPlanCallbackDel del = QueryProductionPlanCallback;
            dataGridView1.BeginInvoke(del, dt, totalCount);
            return;
        }

        public void QueryProductionPlan()
        {
            #region 分页
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            #endregion
            #region where查询
            StringBuilder whereStrbd = new StringBuilder();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                whereStrbd.Append(string.Format(" and VehicleModelCode like '%{0}%' ", textBox1.Text));
            }
            if (!string.IsNullOrEmpty(dateTimePicker1.Text) && !dateTimePicker1.Text.Equals(" "))
            {
                whereStrbd.Append(string.Format(" and ProductionDate>='{0}' ", dateTimePicker1.Value.ToString("yyyy-MM-dd")));
            }
            if (!string.IsNullOrEmpty(dateTimePicker2.Text) && !dateTimePicker2.Text.Equals(" "))
            {
                whereStrbd.Append(string.Format(" and ProductionDate<'{0}' ", dateTimePicker2.Value.AddDays(1).ToString("yyyy-MM-dd")));
            }
            #endregion
            Task.Run(() => QueryProductionPlan(limit,offset,whereStrbd.ToString()));
            return;
        }
        #endregion
        #region 删除计划
        delegate void DeleteRecordsCallbackDel(string errorMessage);
        public void DeleteRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            QueryProductionPlan();
            return;
        }
        public void DeletePlanRecords(string tableName, List<string> ids)
        {
            string errorMessage = string.Empty;
            int result = new ExtractInventoryTool_ProductionPlanBLL().DeleteRecords(tableName, ids, out errorMessage);
            DeleteRecordsCallbackDel del = DeleteRecordsCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
            //MessageBox.Show("删除成功", "Info");
        }
        #endregion
        #region 清空计划
        delegate void ClearRecordsCallbackDel(string errorMessage);
        public void ClearRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            QueryProductionPlan();
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

        private void button4_Click(object sender, EventArgs e)
        {
            InitDateTimePicker();
        }

        private void dateTimePicker1_MouseDown(object sender, MouseEventArgs e)
        {
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            return;
        }

        private void dateTimePicker2_MouseDown(object sender, MouseEventArgs e)
        {
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            return;
        }
    }
}
