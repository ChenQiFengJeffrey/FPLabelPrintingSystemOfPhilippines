using ExtractInventoryTool.ColumnNameConfig;
using ExtractInventoryTool.EditorForm;
using ExtractInventoryTool.ToolForm;
using FPLabelData.Entity;
using LabelPrintDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractInventoryTool.TabForm
{
    public partial class Form_PickUpDemandTab : Form
    {
        #region 私有属性
        private DataTable _planTable = null;
        #endregion
        public Form_PickUpDemandTab()
        {
            InitializeComponent();
            InitPickUpDemandTable(dataGridView1, new int[] { 0 });//PickUpDemand配置
            InitPagerControl();//分页控件
            InitDateTimePicker();
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
        public void InitPickUpDemandTable(DataGridView dv, int[] idAry)
        {
            PickUpDemandConfig demand = new PickUpDemandConfig();
            _planTable = new DataTable();
            _planTable.Columns.Add(demand.Oid, typeof(Int32));//0
            _planTable.Columns.Add(demand.Code, typeof(string));//1
            _planTable.Columns.Add(demand.SupplierCode, typeof(string));//2
            _planTable.Columns.Add(demand.ProductionDate, typeof(DateTime));//3
            _planTable.Columns.Add(demand.DailyUsage, typeof(Int32));//4
            _planTable.Columns.Add(demand.QueryTime, typeof(DateTime));//5
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
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            QueryPickUpDemand();
            return;
        }
        /// <summary>
        /// 导出取货需求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            ExtractPickUpDemand();
        }

        #region 查询取货需求
        /// <summary>
        /// 查询取货需求回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryPickUpDemandCallbackDel(DataTable dt, int totalCount);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryPickUpDemandCallback(DataTable dt, int totalCount)
        {
            _planTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _planTable.Rows.Add(
                    row["Oid"],
                    row["Code"],
                    row["SupplierCode"],
                    row["ProductionDate"],
                    row["DailyUsage"],
                    row["QueryTime"]
                    );
            }
            BindGrid(dataGridView1, _planTable, new int[] { 0 });
            pagerControl1.DrawControl(totalCount);
            return;
        }
        /// <summary>
        /// 查询客户
        /// </summary>
        public void QueryPickUpDemand(string limit, string offset, string whereStr)
        {
            int totalCount = 0;
            DataTable dt = new ExtractInventoryTool_PickUpDemandBLL().QueryPickUpDemand(limit, offset, whereStr, out totalCount);
            QueryPickUpDemandCallbackDel del = QueryPickUpDemandCallback;
            dataGridView1.BeginInvoke(del, dt, totalCount);
            return;
        }

        public void QueryPickUpDemand()
        {
            #region 分页
            string limit = pagerControl1.PageSize.ToString();
            string offset = (pagerControl1.PageIndex - 1).ToString();
            #endregion
            #region where查询
            StringBuilder whereStrbd = new StringBuilder();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                whereStrbd.Append(string.Format(" and Code like '%{0}%' ", textBox1.Text));
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
            Task.Run(() => QueryPickUpDemand(limit, offset, whereStrbd.ToString()));
            return;
        }
        #endregion

        #region 导出取货需求
        public void ExtractPickUpDemand()
        {
            #region where查询
            StringBuilder whereStrbd = new StringBuilder();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                whereStrbd.Append(string.Format(" and Code like '%{0}%' ", textBox1.Text));
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
            
            Task.Run(() => ExtractPickUpDemand(whereStrbd.ToString()));
            //ExtractPickUpDemand(whereStrbd.ToString());
            return;
        }


        public void ExtractPickUpDemand(string whereStr)
        {
            DataTable dt = new ExtractInventoryTool_PickUpDemandBLL().QueryPickUpDemand(whereStr);
            ExtractPickUpDemandCallbackDel del = ExtractPickUpDemandCallback;
            dataGridView1.BeginInvoke(del, dt);
            return;
        }

        /// <summary>
        /// 查询取货需求回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void ExtractPickUpDemandCallbackDel(DataTable dt);
        /// <summary>
        /// 导出Excel回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void ExtractPickUpDemandCallback(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("没有可导出的数据，请重新设置查询条件", "Warning");
            }
            List<ExtractInventoryTool_PickUpDemandDetails> demandList = new List<ExtractInventoryTool_PickUpDemandDetails>();
            foreach (DataRow row in dt.Rows)
            {
                ExtractInventoryTool_PickUpDemandDetails demand = new ExtractInventoryTool_PickUpDemandDetails();
                demand.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                demand.SupplierCode = row["SupplierCode"] == DBNull.Value ? "" : Convert.ToString(row["SupplierCode"]);
                demand.Supplier = row["Supplier"] == DBNull.Value ? "" : Convert.ToString(row["Supplier"]);
                demand.Code = row["Code"] == DBNull.Value ? "" : Convert.ToString(row["Code"]);
                demand.Name = row["Name"] == DBNull.Value ? "" : Convert.ToString(row["Name"]);
                demand.SysInventory = row["SysInventory"] == DBNull.Value ? 0 : Convert.ToInt32(row["SysInventory"]);
                demand.InTransit = row["InTransit"] == DBNull.Value ? 0 : Convert.ToInt32(row["InTransit"]);
                demand.HUB = row["HUB"] == DBNull.Value ? 0 : Convert.ToInt32(row["HUB"]);
                demand.Total = row["Total"] == DBNull.Value ? 0 : Convert.ToInt32(row["Total"]);
                demand.ProductionDate = Convert.ToDateTime(row["ProductionDate"]);
                demand.DailyUsage = row["DailyUsage"] == DBNull.Value ? 0 : Convert.ToInt32(row["DailyUsage"]);
                demandList.Add(demand);
            }
            ExtractToExcel(demandList);
        }

        public void ExtractToExcel(List<ExtractInventoryTool_PickUpDemandDetails> demandList)
        {
            if (demandList == null || demandList.Count == 0)
                return;
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string sourceFile = path + "取货计划.xlsx";
            if (!File.Exists(sourceFile))
            {
                MessageBox.Show("取货计划模板不存在", "Error");
            }
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件保存路径";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            string foldPath = dialog.SelectedPath;
            StringBuilder newExcelName = new StringBuilder();
            newExcelName
                .Append("取货计划")
                .Append(DateTime.Now.ToString("yyyy-MMdd-HHmmss"))
                .Append(".xlsx");
            //File.Copy(sourceFile, Path.Combine(foldPath, newExcelName.ToString()), true);
            LoadingHelper.ShowLoadingScreen("正在导出..");
            new NPOIHelper().WriteToPickUpDemandExcel(sourceFile, Path.Combine(foldPath, newExcelName.ToString()), demandList);
            LoadingHelper.CloseForm();
            MessageBox.Show("导出成功！","Success");
            return;
        }
        #endregion

        private void Form_PickUpDemandTab_Load(object sender, EventArgs e)
        {

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

        private void button4_Click(object sender, EventArgs e)
        {
            InitDateTimePicker();
        }
    }
}
