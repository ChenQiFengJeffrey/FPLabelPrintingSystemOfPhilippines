using DevExpress.XtraReports.UI;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using FPLabelData;
using FPLabelPrintingSystemOfPhilippines.ColumnNameConfig;
using FPLabelPrintingWcfService;
using LabelPrintDAL;
using LabelReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Media;
using System.ServiceModel;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPLabelPrintingSystemOfPhilippines
{
    public partial class Form1 : Form
    {
        static object lockObj = new object();
        private DataTable _packageSetTable = null;
        private DataTable _goodSetTable = null;
        private DataTable _printSetTable = null;
        private DataTable _labelRecordTable = null;
        private DataTable _goodRoTable = null;
        private PrintSet _currentPrintSet = null;
        private List<KeyValuePair<string, string>> _snList = new List<KeyValuePair<string, string>>();
        private List<GoodSet> _currentGoodSetList = null;
        static ServiceHost host = new ServiceHost(typeof(PrintSetService));
        private string _workStation = ConfigurationManager.AppSettings["WorkStation"].Trim();
        private string _supplierName = ConfigurationManager.AppSettings["SupplierName"].Trim();
        private string _supplierCode = ConfigurationManager.AppSettings["SupplierCode"].Trim();
        private string _idStr = string.Empty;
        public Form1()
        {
            InitializeComponent();
            InitPackageSetTable();//包装配置
            InitGoodSetTable();//成品配置
            InitGoodRoTable();//成品RO配置
            InitPrintSetTable();//打印配置
            InitLabelRecordTable();//标签记录
            Task.Run(() => LabelPrintingWCFServiceStar());//启动WCF
            LogHelper.Setup();//启动Log4net
        }

        public void LabelPrintingWCFServiceStar()
        {
            try
            {
                host.Open();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LabelPrintingWCFServiceStar", ex);
                //throw ex;
            }
        }

        /// <summary>
        /// 初始化包装配置
        /// </summary>
        public void InitPackageSetTable()
        {
            PackageSetConfig package = new PackageSetConfig();
            _packageSetTable = new DataTable();
            _packageSetTable.Columns.Add(package.Oid, typeof(int));
            _packageSetTable.Columns.Add(package.FinishedProductVariety, typeof(string));
            _packageSetTable.Columns.Add(package.A, typeof(bool));
            _packageSetTable.Columns.Add(package.B, typeof(bool));
            _packageSetTable.Columns.Add(package.C, typeof(bool));
            _packageSetTable.Columns.Add(package.D, typeof(bool));
            _packageSetTable.Columns.Add(package.E, typeof(bool));
            _packageSetTable.Columns.Add(package.HOME, typeof(bool));
            _packageSetTable.Columns.Add(package.SME, typeof(bool));
            bindGrid(dataGridView4, _packageSetTable, new int[] { 0 });
        }
        /// <summary>
        /// 初始化成品配置
        /// </summary>
        public void InitGoodSetTable()
        {
            GoodSetConfig good = new GoodSetConfig();
            _goodSetTable = new DataTable();
            _goodSetTable.Columns.Add(good.Oid, typeof(string));
            _goodSetTable.Columns.Add(good.FinishedProductNum, typeof(string));
            _goodSetTable.Columns.Add(good.MaterialNum, typeof(string));
            _goodSetTable.Columns.Add(good.MaterialName, typeof(string));
            _goodSetTable.Columns.Add(good.QTY, typeof(int));
            _goodSetTable.Columns.Add(good.FinishedProductID, typeof(string));
            bindGrid(dataGridView1, _goodSetTable, new int[] { 0, 5 });
        }

        public void InitGoodRoTable()
        {
            GoodRoConfig good = new GoodRoConfig();
            _goodRoTable = new DataTable();
            _goodRoTable.Columns.Add(good.Oid, typeof(string));
            _goodRoTable.Columns.Add(good.FinishedProductNum, typeof(string));
            _goodRoTable.Columns.Add(good.Ro, typeof(string));
            _goodRoTable.Columns.Add(good.FinishedProductID, typeof(string));
            bindGrid(dataGridView5, _goodRoTable, new int[] { 0, 3 });
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
            dv.AutoResizeColumns();
            dv.ClearSelection();
        }
        /// <summary>
        /// 初始化打印配置
        /// </summary>
        public void InitPrintSetTable()
        {
            PrintSetConfig print = new PrintSetConfig();
            _printSetTable = new DataTable();
            _printSetTable.Columns.Add(print.Oid, typeof(string));
            _printSetTable.Columns.Add(print.FinishedProductNum, typeof(string));
            _printSetTable.Columns.Add(print.SN, typeof(string));
            _printSetTable.Columns.Add(print.Package, typeof(string));
            _printSetTable.Columns.Add(print.CreateTime, typeof(DateTime));
            bindGrid(dataGridView2, _printSetTable, new int[] { 0 });
        }

        /// <summary>
        /// 初始化标签记录
        /// </summary>
        public void InitLabelRecordTable()
        {
            LabelRecordConfig record = new LabelRecordConfig();
            _labelRecordTable = new DataTable();
            _labelRecordTable.Columns.Add(record.CreateTime, typeof(DateTime));//0
            _labelRecordTable.Columns.Add(record.WorkStation, typeof(string));//1
            _labelRecordTable.Columns.Add(record.RoNumber, typeof(string));//2
            _labelRecordTable.Columns.Add(record.FinishedProductNum, typeof(string));//3
            _labelRecordTable.Columns.Add(record.Oid, typeof(string));//4
            _labelRecordTable.Columns.Add(record.ID, typeof(string));//5
            _labelRecordTable.Columns.Add(record.A, typeof(bool));//6
            _labelRecordTable.Columns.Add(record.B, typeof(bool));//7
            _labelRecordTable.Columns.Add(record.C, typeof(bool));//8
            _labelRecordTable.Columns.Add(record.D, typeof(bool));//9
            _labelRecordTable.Columns.Add(record.E, typeof(bool));//10
            _labelRecordTable.Columns.Add(record.F, typeof(bool));//11
            _labelRecordTable.Columns.Add(record.G, typeof(bool));//12
            _labelRecordTable.Columns.Add(record.H, typeof(bool));//13
            _labelRecordTable.Columns.Add(record.I, typeof(bool));//14
            _labelRecordTable.Columns.Add(record.J, typeof(bool));//15
            _labelRecordTable.Columns.Add(record.K, typeof(bool));//16
            _labelRecordTable.Columns.Add(record.L, typeof(bool));//17
            _labelRecordTable.Columns.Add(record.M, typeof(bool));//18
            _labelRecordTable.Columns.Add(record.HOME, typeof(bool));//19
            _labelRecordTable.Columns.Add(record.SME, typeof(bool));//20
            _labelRecordTable.Columns.Add(record.MSI, typeof(bool));//21
            _labelRecordTable.Columns.Add(record.FTTH, typeof(bool));//22
            _labelRecordTable.Columns.Add(record.MSIVOICEONLY, typeof(bool));//23
            _labelRecordTable.Columns.Add(record.COPPERDATAONLY, typeof(bool));//24
            _labelRecordTable.Columns.Add(record.FTTHDATAONLY, typeof(bool));//25
            _labelRecordTable.Columns.Add(record.FTTHNONWIFI, typeof(bool));//26
            _labelRecordTable.Columns.Add(record.FTTHNONWIFIDATAONLY, typeof(bool));//27
            _labelRecordTable.Columns.Add(record.ONU, typeof(string));//28
            _labelRecordTable.Columns.Add(record.VVDSL, typeof(string));//29
            _labelRecordTable.Columns.Add(record.TELSET, typeof(string));//30
            _labelRecordTable.Columns.Add(record.BIZBOX, typeof(string));//31
            _labelRecordTable.Columns.Add(record.Barcode, typeof(string));//32
            _labelRecordTable.Columns.Add(record.GoodList, typeof(string));//33
            
            bindGrid(dataGridView3, _labelRecordTable, new int[] { 4 });
        }
        /// <summary>
        /// Form1加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 显示Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navBarControl1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            DevExpress.XtraNavBar.ICollectionItem item = e.Link as DevExpress.XtraNavBar.ICollectionItem;
            switch (item.ItemName.Trim().ToLower())
            {
                case "navbaritem1"://成品配置
                    xtraTabPage1.PageVisible = true;
                    xtraTabPage1.Show();
                    break;
                case "navbaritem2"://打印配置
                    xtraTabPage2.PageVisible = true;
                    xtraTabPage2.Show();
                    break;
                case "navbaritem3"://标签打印
                    xtraTabPage3.PageVisible = true;
                    xtraTabPage3.Show();
                    break;
                case "navbaritem4"://标签记录
                    xtraTabPage4.PageVisible = true;
                    xtraTabPage4.Show();
                    break;
                case "navbaritem5"://标签记录
                    xtraTabPage5.PageVisible = true;
                    xtraTabPage5.Show();
                    break;
                case "navbaritem6"://成品RO配置
                    xtraTabPage6.PageVisible = true;
                    xtraTabPage6.Show();
                    break;
                default:
                    break;
            }
            return;
        }
        /// <summary>
        /// 关闭Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            ClosePageButtonEventArgs arg = e as ClosePageButtonEventArgs;
            (arg.Page as XtraTabPage).PageVisible = false;
        }
        /// <summary>
        /// 成品配置_查询/刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => QueryGoodSet());
            //QueryGoodSet();
            return;
        }
        delegate void QueryGoodSetCallbackDel(DataTable dt);
        public void QueryGoodSet()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append(@"select n0.*,n1.FinishedProductNum as FinishedProductName from GoodSet n0 left join PrintSet n1 on n0.FinishedProductNum=n1.Oid");
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());

                    QueryGoodSetCallbackDel del = QueryGoodSetCallback;
                    dataGridView1.BeginInvoke(del, dt);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Ro查询/刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {
            Task.Run(() => QueryGoodRoSet());
            return;
        }

        delegate void QueryGoodRoSetCallbackDel(DataTable dt);
        /// <summary>
        /// 查询成品Ro#
        /// </summary>
        public void QueryGoodRoSet()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append(@"select n0.*,n1.FinishedProductNum as FinishedProductName from RoSet n0 left join PrintSet n1 on n0.FinishedProductNum=n1.Oid");
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());

                    QueryGoodRoSetCallbackDel del = QueryGoodRoSetCallback;
                    dataGridView5.BeginInvoke(del, dt);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="dt"></param>
        public void QueryGoodSetCallback(DataTable dt)
        {
            _goodSetTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _goodSetTable.Rows.Add(
                    row["Oid"],
                    row["FinishedProductName"],
                    row["MaterialNum"],
                    row["MaterialName"],
                    row["QTY"],
                    row["FinishedProductNum"]
                    );
            }
            bindGrid(dataGridView1, _goodSetTable, new int[] { 0, 5 });
            return;
        }
        public void QueryGoodRoSetCallback(DataTable dt)
        {
            _goodRoTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _goodRoTable.Rows.Add(
                    row["Oid"],
                    row["FinishedProductName"],
                    row["RoNumber"],
                    row["FinishedProductNum"]
                    );
            }
            bindGrid(dataGridView5, _goodRoTable, new int[] { 0, 3 });
            return;
        }

        /// <summary>
        /// 成品配置_添加/更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            GoodSet good = null;
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count > 0)
            {
                row = rowCollection[0];
            }
            if (row != null && row.Cells[0].Value != null)
            {
                good = new GoodSet();
                int oid = 0;
                good.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                int fpOid = 0;
                good.FinishedProductNum = int.TryParse(row.Cells[5].Value.ToString().Trim(), out fpOid) ? fpOid : 0;
                good.MaterialNum = row.Cells[2].Value.ToString().Trim();
                good.MaterialName = row.Cells[3].Value.ToString().Trim();
                int qty = 0;
                good.QTY = int.TryParse(row.Cells[4].Value.ToString().Trim(), out qty) ? qty : 0;
            }
            Form_GoodEditor editor = new Form_GoodEditor(good);
            editor.ShowDialog(this);
            return;
        }

        /// <summary>
        /// Ro更新/添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {

            {
                RoSet ro = null;
                DataGridViewSelectedRowCollection rowCollection = dataGridView5.SelectedRows;
                DataGridViewRow row = null;
                if (rowCollection.Count > 0)
                {
                    row = rowCollection[0];
                }
                if (row != null && row.Cells[0].Value != null)
                {
                    ro = new RoSet();
                    int oid = 0;
                    ro.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                    int fpOid = 0;
                    ro.FinishedProductNum = int.TryParse(row.Cells[3].Value.ToString().Trim(), out fpOid) ? fpOid : 0;
                    ro.RoNumber = row.Cells[2].Value.ToString().Trim();
                }
                Form_GoodRoEditor editor = new Form_GoodRoEditor(ro);
                editor.ShowDialog(this);
                return;
            }
        }

        /// <summary>
        /// 包装配置_添加/更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //var btn = sender as Button;
            PackageSet package = null;
            DataGridViewSelectedRowCollection rowCollection = dataGridView4.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count > 0)
            {
                row = rowCollection[0];
            }
            if (row != null && row.Cells[0].Value != null)
            {
                package = new PackageSet();
                int oid = 0;
                package.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                package.FinishedProductVariety = row.Cells[1].Value.ToString().Trim();
                bool a = false;
                package.A = bool.TryParse(row.Cells[2].Value.ToString().Trim(), out a) ? a : false;
                bool b = false;
                package.B = bool.TryParse(row.Cells[3].Value.ToString().Trim(), out b) ? b : false;
                bool c = false;
                package.C = bool.TryParse(row.Cells[4].Value.ToString().Trim(), out c) ? c : false;
                bool d = false;
                package.D = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out d) ? d : false;
                bool pe = false;
                package.E = bool.TryParse(row.Cells[6].Value.ToString().Trim(), out pe) ? pe : false;
                bool f = false;
                package.F = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out f) ? f : false;
                bool g = false;
                package.G = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out g) ? g : false;
                bool h = false;
                package.H = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out h) ? h : false;
                bool i = false;
                package.I = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out i) ? i : false;
                bool j = false;
                package.J = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out j) ? j : false;
                bool k = false;
                package.K = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out k) ? k : false;
                bool l = false;
                package.L = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out l) ? l : false;
                bool m = false;
                package.M = bool.TryParse(row.Cells[5].Value.ToString().Trim(), out m) ? m : false;
                bool home = false;
                package.HOME = bool.TryParse(row.Cells[7].Value.ToString().Trim(), out home) ? home : false;
                bool sme = false;
                package.SME = bool.TryParse(row.Cells[8].Value.ToString().Trim(), out sme) ? sme : false;
            }
            Form_PackageEditor editor = new Form_PackageEditor(package);
            editor.ShowDialog(this);
            return;
        }
        /// <summary>
        /// 包装配置_查询/刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(() => QueryPackage());
            return;
            //QueryPackage();
        }
        delegate void QueryPackageCallbackDel(DataTable dt);
        public void QueryPackage()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from PackageSet order by Oid desc");
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());

                    QueryPackageCallbackDel del = QueryPackageCallback;
                    dataGridView4.BeginInvoke(del, dt);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void QueryPackageCallback(DataTable dt)
        {
            _packageSetTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _packageSetTable.Rows.Add(
                    row["Oid"],
                    row["FinishedProductVariety"],
                    row["A"],
                    row["B"],
                    row["C"],
                    row["D"],
                    row["E"],
                    row["HOME"],
                    row["SME"]
                    );
            }
            bindGrid(dataGridView4, _packageSetTable, new int[] { 0 });
            return;
        }
        /// <summary>
        /// 打印配置_查询/刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            Task.Run(() => QueryPrintSet());
            return;
        }
        delegate void QueryPrintSetCallbackDel(DataTable dt);
        public void QueryPrintSet()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from PrintSet order by Oid desc");
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());

                    QueryPrintSetCallbackDel del = QueryPrintSetCallback;
                    dataGridView4.BeginInvoke(del, dt);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void QueryPrintSetCallback(DataTable dt)
        {
            _printSetTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _printSetTable.Rows.Add(
                    row["Oid"],
                    row["FinishedProductNum"],
                    row["SN"],
                    row["Package"],
                    row["CreateTime"]
                    );
            }
            bindGrid(dataGridView2, _printSetTable, new int[] { 0 });
            return;
        }
        /// <summary>
        /// 打印配置_添加/更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            PrintSet print = null;
            DataGridViewSelectedRowCollection rowCollection = dataGridView2.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count > 0)
            {
                row = rowCollection[0];
            }
            if (row != null && row.Cells[0].Value != null)
            {
                print = new PrintSet();
                int oid = 0;
                print.Oid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                print.FinishedProductNum = row.Cells[1].Value.ToString().Trim();
                print.SN = row.Cells[2].Value.ToString().Trim();
                print.Package = row.Cells[3].Value.ToString().Trim();
                DateTime createTime = new DateTime();
                print.CreateTime = DateTime.TryParse(row.Cells[4].Value.ToString().Trim(), out createTime) ? createTime : DateTime.MinValue;
            }
            Form_PrintSetEditor editor = new Form_PrintSetEditor(print);
            editor.ShowDialog(this);
            return;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            #region 这里做流程控制，在同一个线程中操作
            try
            {
                //1.获取成品料号
                TextBox tb = sender as TextBox;
                string text = tb.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    Speecher("Finished Product Number isn't null");
                    return;
                }
                //2、获取打印配置
                PrintSetBLL psbll = new PrintSetBLL();
                DataTable psdt = psbll.GetPrintSetByFPNum(text);
                if (psdt == null || psdt.Rows.Count == 0)
                {
                    Speecher("Print Config is null");
                    return;
                }
                //3、获取成品配置
                GoodSetBLL gsbll = new GoodSetBLL();
                DataTable gsdt = gsbll.GetGoodSetByFPNum(text);
                if (gsdt == null || gsdt.Rows.Count == 0)
                {
                    Speecher("Good Config is null");
                    return;
                }
                button13.BackColor = Color.Green;
                List<PrintSet> printsetList = PrintSet.DataTableToList(psdt);
                List<GoodSet> goodsetList = GoodSet.DataTableToList(gsdt);
                _currentPrintSet = null;
                _currentPrintSet = printsetList.First();
                _currentGoodSetList = null;
                _currentGoodSetList = goodsetList;
                GotoNextTextBox(FinishedProductNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }

        /// <summary>
        /// 标签打印_手动打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (_currentPrintSet == null)
            {
                Speecher("Print Config is null");
                return;
            }
            if (_currentGoodSetList == null)
            {
                Speecher("Good Config is null");
                return;
            }
            #region 自动打印成品标签
            FinishedProductLabelDTO dto = GetFinishedProductLabelDTO();
            List<FinishedProductLabelDTO> dtoList = new List<FinishedProductLabelDTO>();
            dtoList.Add(dto);
            XtraReport1 label = new XtraReport1();
            label.DataSource = dtoList;
            label.PrintingSystem.ShowMarginsWarning = false;
            label.PrintingSystem.ShowPrintStatusDialog = false;
            label.PrintingSystem.EndPrint += PrintingSystem_EndPrint;
            label.PaperName = DateTime.Now.ToString() + dto.ID;
            ReportPrintTool tool = new ReportPrintTool(label);
            tool.Print();
            #endregion
        }
        /// <summary>
        /// 标签打印_清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            LabelPrintClear();
        }
        public void LabelPrintClear()
        {
            _currentPrintSet = null;
            _currentGoodSetList = null;
            _snList.Clear();
            _idStr = "";
            button13.BackColor = Color.Red;
            FinishedProductNum.Clear();
            FinishedProductNum.ReadOnly = false;
            ONU.Clear();
            ONU.ReadOnly = true;
            VVDSL.Clear();
            VVDSL.ReadOnly = true;
            TELSET.Clear();
            TELSET.ReadOnly = true;
            BIZBOX.Clear();
            BIZBOX.ReadOnly = true;
            FinishedProductNum.Focus();
        }
        private void xtraTabPage3_VisibleChanged(object sender, EventArgs e)
        {
            LabelPrintClear();
        }
        public void GotoNextTextBox(TextBox box)
        {
            int index = 0;
            SN currentSN = _currentPrintSet.SNList.FirstOrDefault(p => p.Name.Trim().ToUpper()
                 .Equals(box.Name.Trim().ToUpper()));
            if (currentSN != null && !string.IsNullOrWhiteSpace(currentSN.Code))
            {
                Regex regex = new Regex(currentSN.Code);
                if (!regex.IsMatch(box.Text))
                {
                    Speecher("Scan SN Label failed");
                    box.Text = "";
                    return;
                }
            }
            index = currentSN == null ? 0 : currentSN.Order;
            bool isNext = index == 0 ? true : false;
            box.ReadOnly = true;
            foreach (var sn in _currentPrintSet.SNList)
            {
                if (isNext)
                {
                    Control[] controlAry = box.Parent.Controls.Find(sn.Name, true);
                    if (controlAry.Length == 0)
                    {
                        Speecher("S N Config is wrong");
                        return;
                    }
                    var nextBox = controlAry[0] as TextBox;
                    nextBox.ReadOnly = false;
                    nextBox.Focus();
                    return;
                }
                isNext = sn.Order == index;
            }
            #region 自动打印成品标签
            FinishedProductLabelDTO dto = GetFinishedProductLabelDTO();
            List<FinishedProductLabelDTO> dtoList = new List<FinishedProductLabelDTO>();
            dtoList.Add(dto);
            XtraReport1 label = new XtraReport1();
            label.DataSource = dtoList;
            label.PrintingSystem.ShowMarginsWarning = false;
            label.PrintingSystem.ShowPrintStatusDialog = false;
            label.PrintingSystem.EndPrint += PrintingSystem_EndPrint;
            label.PaperName = DateTime.Now.ToString() + dto.ID;
            ReportPrintTool tool = new ReportPrintTool(label);
            tool.Print();
            #endregion
        }
        /// <summary>
        /// 组成dto,给打印模板当数据源
        /// </summary>
        /// <returns></returns>
        public FinishedProductLabelDTO GetFinishedProductLabelDTO()
        {
            FinishedProductLabelDTO result = new FinishedProductLabelDTO();
            result.ID = string.IsNullOrWhiteSpace(_idStr) ? "F" + GeneratedGUID.GuidTo19String() : _idStr;
            _idStr = result.ID;
            #region Package
            result.FinishedProductNum = _currentPrintSet.FinishedProductNum;
            Package a = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("A"));
            result.A = a == null ? false : a.IsChecked;
            Package b = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("B"));
            result.B = b == null ? false : b.IsChecked;
            Package c = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("C"));
            result.C = c == null ? false : c.IsChecked;
            Package d = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("D"));
            result.D = d == null ? false : d.IsChecked;
            Package e = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("E"));
            result.E = e == null ? false : e.IsChecked;
            Package f = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("F"));
            result.F = f == null ? false : f.IsChecked;
            Package g = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("G"));
            result.G = g == null ? false : g.IsChecked;
            Package h = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("H"));
            result.H = h == null ? false : h.IsChecked;
            Package i = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("I"));
            result.I = i == null ? false : i.IsChecked;
            Package j = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("J"));
            result.J = j == null ? false : j.IsChecked;
            Package pk = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("K"));
            result.K = pk == null ? false : pk.IsChecked;
            Package l = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("L"));
            result.L = l == null ? false : l.IsChecked;
            Package m = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("M"));
            result.M = m == null ? false : m.IsChecked;
            Package home = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("HOME"));
            result.HOME = home == null ? false : home.IsChecked;
            Package sme = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("SME"));
            result.SME = sme == null ? false : sme.IsChecked;
            #endregion
            #region PackageType
            Package msi = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("MSI"));
            result.MSI = msi == null ? false : msi.IsChecked;
            Package ftth = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("FTTH"));
            result.FTTH = ftth == null ? false : ftth.IsChecked;
            Package msivoiceonly = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("SMSIVOICEONLYME"));
            result.MSIVOICEONLY = msivoiceonly == null ? false : msivoiceonly.IsChecked;
            Package copperdataonly = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("SCOPPERDATAONLYME"));
            result.COPPERDATAONLY = copperdataonly == null ? false : copperdataonly.IsChecked;
            Package ftthdataonly = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("FTTHDATAONLY"));
            result.FTTHDATAONLY = ftthdataonly == null ? false : ftthdataonly.IsChecked;
            Package ftthnonwifi = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("FTTHNONWIFI"));
            result.FTTHNONWIFI = ftthnonwifi == null ? false : ftthnonwifi.IsChecked;
            Package ftthnonwifidataonly = _currentPrintSet.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("FTTHNONWIFIDATAONLY"));
            result.FTTHNONWIFIDATAONLY = ftthnonwifidataonly == null ? false : ftthnonwifidataonly.IsChecked;
            #endregion
            #region SN
            result.ONU = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("ONU")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("ONU")).Value : "";
            result.VVDSL = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("VDSL")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("VDSL")).Value : "";
            result.TELSET = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("TELSET")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("TELSET")).Value : "";
            result.BIZBOX = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("BIZBOX")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("BIZBOX")).Value : "";
            #endregion
            #region Good
            result.GoodList = _currentGoodSetList;
            #endregion
            #region Barcode
            StringBuilder barcodeStrbd = new StringBuilder();
            barcodeStrbd.Append("(")
                        .Append("").Append("|")//库位
                        .Append("").Append("|")//???
                        .Append("").Append("|")//ASN
                        .Append(_currentPrintSet.FinishedProductNum).Append("|")//物料号
                        .Append("").Append("|")//???
                        .Append("1").Append("|")//数量
                        .Append("").Append("|")//批次
                        .Append(_supplierName).Append("|")//供应商名称
                        .Append(_supplierCode).Append("|")//供应商编码
                        .Append("").Append("|")//箱数
                        .Append("").Append("|")//总箱数
                        .Append(result.ID).Append("|")//唯一码
                        .Append(result.ONU).Append("|")//sunumber|ONU
                        .Append(result.VVDSL).Append("|")//productionlot|VDSL
                        .Append(result.TELSET).Append("|")//rawmateriallot|TELSET
                        .Append(result.BIZBOX)             //heatnumber|BIZBOX
                        .Append(")");
            result.Barcode = barcodeStrbd.ToString();
            #endregion
            return result;
        }
        private void PrintingSystem_EndPrint(object sender, EventArgs e)
        {

            //异步记录打印记录
            FinishedProductLabelDTO dto = GetFinishedProductLabelDTO();
            dto.WorkStation = _workStation;

            List<FinishedProductLabelDTO> dtoList = new List<FinishedProductLabelDTO>();
            dtoList.Add(dto);
            //InsertLabelRecord(dtoList);
            Task.Run(() => InsertLabelRecord(dtoList));
            LabelPrintClear();
            return;
        }

        private void ONU_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the Textbox is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("ONU", box.Text));
            GotoNextTextBox(ONU);
        }

        private void VVDSL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the Textbox is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("VDSL", box.Text));
            GotoNextTextBox(VVDSL);
        }

        private void TELSET_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the Textbox is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("TELSET", box.Text));
            GotoNextTextBox(TELSET);
        }

        private void BIZBOX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the Textbox is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("BIZBOX", box.Text));
            GotoNextTextBox(BIZBOX);
        }
        /// <summary>
        /// 语音播报
        /// </summary>
        /// <param name="text"></param>
        public void Speecher(string text)
        {
            //using (SpeechSynthesizer speech = new SpeechSynthesizer())
            //{
            //    speech.Rate = 0;  //语速
            //    speech.Volume = 100;  //音量
            //    speech.Speak(text);
            //}
            //SoundPlayer play = new SoundPlayer();
            //play.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"Sound\FinishedProductNumNull.wav";
            //play.Load();  //加载声音
            //play.Play(); //播放
            MessageBox.Show(text, "Warning");
        }
        /// <summary>
        /// 开始，手动获取打印配置和成品配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            #region 这里做流程控制，在同一个线程中操作
            try
            {
                //1.获取成品料号
                string text = FinishedProductNum.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    Speecher("Finished Product Number is null");
                    return;
                }
                //2、获取打印配置
                PrintSetBLL psbll = new PrintSetBLL();
                DataTable psdt = psbll.GetPrintSetByFPNum(text);
                if (psdt == null || psdt.Rows.Count == 0)
                {
                    Speecher("Print Config is null");
                    return;
                }
                //3、获取成品配置
                GoodSetBLL gsbll = new GoodSetBLL();
                DataTable gsdt = gsbll.GetGoodSetByFPNum(text);
                if (gsdt == null || gsdt.Rows.Count == 0)
                {
                    Speecher("Good Config is null");
                    return;
                }
                button13.BackColor = Color.Green;
                List<PrintSet> printsetList = PrintSet.DataTableToList(psdt);
                List<GoodSet> goodsetList = GoodSet.DataTableToList(gsdt);
                _currentPrintSet = null;
                _currentPrintSet = printsetList.First();
                _currentGoodSetList = null;
                _currentGoodSetList = goodsetList;
                GotoNextTextBox(FinishedProductNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
        /// <summary>
        /// 添加标签记录
        /// </summary>
        /// <param name="dto"></param>
        public void InsertLabelRecord(List<FinishedProductLabelDTO> dtoList)
        {
            lock (lockObj)
            {
                try
                {
                    if (dtoList == null || dtoList.Count == 0) return;
                    foreach (var dto in dtoList)
                    {
                        StringBuilder noQueryStrbd = new StringBuilder();
                        List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                        SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,dto.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@ID", DbType.String,dto.ID),
                            SQLiteHelper.MakeSQLiteParameter("@A", DbType.Boolean,dto.A),
                            SQLiteHelper.MakeSQLiteParameter("@B", DbType.Boolean,dto.B),
                            SQLiteHelper.MakeSQLiteParameter("@C", DbType.Boolean,dto.C),
                            SQLiteHelper.MakeSQLiteParameter("@D", DbType.Boolean,dto.D),
                            SQLiteHelper.MakeSQLiteParameter("@E", DbType.Boolean,dto.E),
                            SQLiteHelper.MakeSQLiteParameter("@F", DbType.Boolean,dto.F),
                            SQLiteHelper.MakeSQLiteParameter("@G", DbType.Boolean,dto.G),
                            SQLiteHelper.MakeSQLiteParameter("@H", DbType.Boolean,dto.H),
                            SQLiteHelper.MakeSQLiteParameter("@I", DbType.Boolean,dto.I),
                            SQLiteHelper.MakeSQLiteParameter("@J", DbType.Boolean,dto.J),
                            SQLiteHelper.MakeSQLiteParameter("@K", DbType.Boolean,dto.K),
                            SQLiteHelper.MakeSQLiteParameter("@L", DbType.Boolean,dto.L),
                            SQLiteHelper.MakeSQLiteParameter("@M", DbType.Boolean,dto.M),
                            SQLiteHelper.MakeSQLiteParameter("@HOME", DbType.Boolean,dto.HOME),
                            SQLiteHelper.MakeSQLiteParameter("@SME", DbType.Boolean,dto.SME),
                            SQLiteHelper.MakeSQLiteParameter("@MSI", DbType.Boolean,dto.MSI),
                            SQLiteHelper.MakeSQLiteParameter("@FTTH", DbType.Boolean,dto.FTTH),
                            SQLiteHelper.MakeSQLiteParameter("@MSIVOICEONLY", DbType.Boolean,dto.MSIVOICEONLY),
                            SQLiteHelper.MakeSQLiteParameter("@COPPERDATAONLY", DbType.Boolean,dto.COPPERDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHDATAONLY", DbType.Boolean,dto.FTTHDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHNONWIFI", DbType.Boolean,dto.FTTHNONWIFI),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHNONWIFIDATAONLY", DbType.Boolean,dto.FTTHNONWIFIDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@ONU", DbType.String,dto.ONU),
                            SQLiteHelper.MakeSQLiteParameter("@VDSL", DbType.String,dto.VVDSL),
                            SQLiteHelper.MakeSQLiteParameter("@TELSET", DbType.String,dto.TELSET),
                            SQLiteHelper.MakeSQLiteParameter("@BIZBOX", DbType.String,dto.BIZBOX),
                            SQLiteHelper.MakeSQLiteParameter("@Barcode", DbType.String,dto.Barcode),
                            SQLiteHelper.MakeSQLiteParameter("@GoodList", DbType.String,JsonConvert.SerializeObject(dto.GoodList)),
                            SQLiteHelper.MakeSQLiteParameter("@CreateTime", DbType.DateTime,DateTime.Now),
                            SQLiteHelper.MakeSQLiteParameter("@WorkStation", DbType.String,dto.WorkStation),
                            SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.String,dto.FinishedProductNum)
                            };
                        paramList.Add(parameter);
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into LabelRecord (ID,A,B,C,D,E,F,G,H,I,J,K,L,M,HOME,SME,MSI,FTTH,MSIVOICEONLY,COPPERDATAONLY,FTTHDATAONLY,FTTHNONWIFI,FTTHNONWIFIDATAONLY,ONU,VDSL,TELSET,BIZBOX,Barcode,GoodList,CreateTime,WorkStation,FinishedProductNum) ")
                            .Append(@"values ( ")
                            .Append(@"@ID,@A,@B,@C,@D,@E,@F,@G,@H,@I,@J,@K,@L,@M,@HOME,@SME,@MSI,@FTTH,@MSIVOICEONLY,@COPPERDATAONLY,@FTTHDATAONLY,@FTTHNONWIFI,@FTTHNONWIFIDATAONLY,@ONU,@VDSL,@TELSET,@BIZBOX,@Barcode,@GoodList,@CreateTime,@WorkStation,@FinishedProductNum ")
                            .Append(@")");
                        new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        delegate void QueryLabelRecordCallbackDel(DataTable dt);
        public void QueryLabelRecord(string id, string begintime, string endtime)
        {
            lock (lockObj)
            {
                try
                {
                    List<SQLiteParameter> paramList = new List<SQLiteParameter>();
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from LabelRecord where 1=1 ");
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        queryStrbd.Append(" and ID=@ID");
                        paramList.Add(SQLiteHelper.MakeSQLiteParameter("@ID", DbType.String, id));
                    }
                    if (!string.IsNullOrWhiteSpace(begintime))
                    {
                        queryStrbd.Append(" and CreateTime>=@begintime");
                        paramList.Add(SQLiteHelper.MakeSQLiteParameter("@begintime", DbType.String, begintime));
                    }
                    if (!string.IsNullOrWhiteSpace(endtime))
                    {
                        queryStrbd.Append(" and CreateTime<=@endtime");
                        paramList.Add(SQLiteHelper.MakeSQLiteParameter("@endtime", DbType.String, endtime));
                    }
                    queryStrbd.Append(" order by Oid desc");
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString(), paramList.ToArray());

                    QueryLabelRecordCallbackDel del = QueryLabelRecordCallback;
                    dataGridView4.BeginInvoke(del, dt);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void QueryLabelRecordCallback(DataTable dt)
        {
            _labelRecordTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _labelRecordTable.Rows.Add(
                    row["CreateTime"],
                    row["WorkStation"],
                    row["RoNumber"],
                    row["FinishedProductNum"],
                    row["Oid"],
                    row["ID"],
                    row["A"],
                    row["B"],
                    row["C"],
                    row["D"],
                    row["E"],
                    row["F"],
                    row["G"],
                    row["H"],
                    row["I"],
                    row["J"],
                    row["K"],
                    row["L"],
                    row["M"],
                    row["HOME"],
                    row["SME"],
                    row["MSI"],
                    row["FTTH"],
                    row["MSIVOICEONLY"],
                    row["COPPERDATAONLY"],
                    row["FTTHDATAONLY"],
                    row["FTTHNONWIFI"],
                    row["FTTHNONWIFIDATAONLY"],
                    row["ONU"],
                    row["VDSL"],
                    row["TELSET"],
                    row["BIZBOX"],
                    row["Barcode"],
                    row["GoodList"]
                    );
            }
            bindGrid(dataGridView3, _labelRecordTable, new int[] { 4 });
            return;
        }
        /// <summary>
        /// 标签记录_查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            //QueryLabelRecord(id, "", "");
            Task.Run(() => QueryLabelRecord(id, "", ""));
            return;
        }

        private void xtraTabPage4_VisibleChanged(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 标签记录_打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView3.SelectedRows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("There aren't records selected");
            }

            foreach (DataGridViewRow row in rowCollection)
            {
                List<FinishedProductLabelDTO> dtoList = new List<FinishedProductLabelDTO>();
                FinishedProductLabelDTO dto = new FinishedProductLabelDTO();
                dto.WorkStation = row.Cells[1].Value.ToString();
                dto.FinishedProductNum = row.Cells[2].Value.ToString();
                dto.Oid = row.Cells[3].Value.ToString();
                dto.ID = row.Cells[4].Value.ToString();
                dto.A = Boolean.Parse(row.Cells[5].Value.ToString());
                dto.B = Boolean.Parse(row.Cells[6].Value.ToString());
                dto.C = Boolean.Parse(row.Cells[7].Value.ToString());
                dto.D = Boolean.Parse(row.Cells[8].Value.ToString());
                dto.E = Boolean.Parse(row.Cells[9].Value.ToString());
                dto.F = Boolean.Parse(row.Cells[10].Value.ToString());
                dto.G = Boolean.Parse(row.Cells[11].Value.ToString());
                dto.H = Boolean.Parse(row.Cells[12].Value.ToString());
                dto.I = Boolean.Parse(row.Cells[13].Value.ToString());
                dto.J = Boolean.Parse(row.Cells[14].Value.ToString());
                dto.K = Boolean.Parse(row.Cells[15].Value.ToString());
                dto.L = Boolean.Parse(row.Cells[16].Value.ToString());
                dto.M = Boolean.Parse(row.Cells[17].Value.ToString());
                dto.HOME = Boolean.Parse(row.Cells[18].Value.ToString());
                dto.SME = Boolean.Parse(row.Cells[19].Value.ToString());
                dto.MSI = Boolean.Parse(row.Cells[20].Value.ToString());
                dto.FTTH = Boolean.Parse(row.Cells[21].Value.ToString());
                dto.MSIVOICEONLY = Boolean.Parse(row.Cells[22].Value.ToString());
                dto.COPPERDATAONLY = Boolean.Parse(row.Cells[23].Value.ToString());
                dto.FTTHDATAONLY = Boolean.Parse(row.Cells[24].Value.ToString());
                dto.FTTHNONWIFI = Boolean.Parse(row.Cells[25].Value.ToString());
                dto.FTTHNONWIFIDATAONLY = Boolean.Parse(row.Cells[26].Value.ToString());
                dto.ONU = row.Cells[27].Value.ToString();
                dto.VVDSL = row.Cells[28].Value.ToString();
                dto.TELSET = row.Cells[29].Value.ToString();
                dto.BIZBOX = row.Cells[30].Value.ToString();
                dto.Barcode = row.Cells[31].Value.ToString();
                dto.GoodList = JsonConvert.DeserializeObject<List<GoodSet>>(row.Cells[32].Value.ToString());
                dtoList.Add(dto);
                XtraReport1 label = new XtraReport1();
                label.DataSource = dtoList;
                label.PrintingSystem.ShowMarginsWarning = false;
                label.PrintingSystem.ShowPrintStatusDialog = false;
                ReportPrintTool tool = new ReportPrintTool(label);
                tool.Print();
            }
        }
        /// <summary>
        /// 成品配置_删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("There aren't records selected");
            }
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in rowCollection)
            {
                ids.Add(row.Cells[0].Value.ToString());
            }
            DeleteRecords("GoodSet", ids);
        }
        /// <summary>
        /// 成品Ro_删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView5.SelectedRows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("There aren't records selected");
            }
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in rowCollection)
            {
                ids.Add(row.Cells[0].Value.ToString());
            }
            DeleteRecords("RoSet", ids);
        }
        /// <summary>
        /// 打印配置_删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView2.SelectedRows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("There aren't records selected");
            }
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in rowCollection)
            {
                ids.Add(row.Cells[0].Value.ToString());
            }
            DeleteRecords("PrintSet", ids);
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
        delegate void DeleteRecordsCallbackDel(string tableName);
        public void DeleteRecords(string tableName, List<string> ids)
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder deleteStrbd = new StringBuilder();
                    deleteStrbd.Append("delete from ")
                        .Append(tableName)
                        .Append(" where Oid in ( ");
                    foreach (string id in ids)
                    {
                        deleteStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    deleteStrbd.Remove(deleteStrbd.Length - 1, 1);
                    deleteStrbd.Append(" ) ");
                    int result = new SQLiteHelper().ExecuteNonQuery(deleteStrbd.ToString());
                    if (result > 0)
                    {
                        switch (tableName)
                        {
                            case "GoodSet":
                                QueryGoodSet();
                                break;
                            case "PrintSet":
                                QueryPrintSet();
                                break;
                            case "LabelRecord":
                                string id = textBox1.Text;
                                QueryLabelRecord(id, "", "");
                                break;
                            case "RoSet":
                                QueryGoodRoSet();
                                break;
                            default:
                                break;
                        }
                    }
                    //DeleteRecordsCallbackDel del = DeleteRecordsCallback;
                    //xtraTabControl1.BeginInvoke(del, tableName);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DeleteRecordsCallback(string name)
        {

            MessageBox.Show("The records are deleted!", "Info");
        }
        /// <summary>
        /// 标签记录_导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            DataGridViewRowCollection rowCollection = dataGridView3.Rows;
            if (rowCollection.Count == 0)
            {
                MessageBox.Show("There aren't records selected");
            }
            List<FinishedProductLabelDTO> dtoList = new List<FinishedProductLabelDTO>();
            foreach (DataGridViewRow row in rowCollection)
            {
                FinishedProductLabelDTO dto = new FinishedProductLabelDTO();
                dto.WorkStation = row.Cells[1].Value.ToString();
                dto.FinishedProductNum = row.Cells[2].Value.ToString();
                dto.Oid = row.Cells[3].Value.ToString();
                dto.ID = row.Cells[4].Value.ToString();
                dto.A = Boolean.Parse(row.Cells[5].Value.ToString());
                dto.B = Boolean.Parse(row.Cells[6].Value.ToString());
                dto.C = Boolean.Parse(row.Cells[7].Value.ToString());
                dto.D = Boolean.Parse(row.Cells[8].Value.ToString());
                dto.E = Boolean.Parse(row.Cells[9].Value.ToString());
                dto.F = Boolean.Parse(row.Cells[10].Value.ToString());
                dto.G = Boolean.Parse(row.Cells[11].Value.ToString());
                dto.H = Boolean.Parse(row.Cells[12].Value.ToString());
                dto.I = Boolean.Parse(row.Cells[13].Value.ToString());
                dto.J = Boolean.Parse(row.Cells[14].Value.ToString());
                dto.K = Boolean.Parse(row.Cells[15].Value.ToString());
                dto.L = Boolean.Parse(row.Cells[16].Value.ToString());
                dto.M = Boolean.Parse(row.Cells[17].Value.ToString());
                dto.HOME = Boolean.Parse(row.Cells[18].Value.ToString());
                dto.SME = Boolean.Parse(row.Cells[19].Value.ToString());
                dto.MSI = Boolean.Parse(row.Cells[20].Value.ToString());
                dto.FTTH = Boolean.Parse(row.Cells[21].Value.ToString());
                dto.MSIVOICEONLY = Boolean.Parse(row.Cells[22].Value.ToString());
                dto.COPPERDATAONLY = Boolean.Parse(row.Cells[23].Value.ToString());
                dto.FTTHDATAONLY = Boolean.Parse(row.Cells[24].Value.ToString());
                dto.FTTHNONWIFI = Boolean.Parse(row.Cells[25].Value.ToString());
                dto.FTTHNONWIFIDATAONLY = Boolean.Parse(row.Cells[26].Value.ToString());
                dto.ONU = row.Cells[27].Value.ToString();
                dto.VVDSL = row.Cells[28].Value.ToString();
                dto.TELSET = row.Cells[29].Value.ToString();
                dto.BIZBOX = row.Cells[30].Value.ToString();
                dto.Barcode = row.Cells[31].Value.ToString();
                dto.GoodList = JsonConvert.DeserializeObject<List<GoodSet>>(row.Cells[32].Value.ToString());
                dtoList.Add(dto);
            }
            XtraReport2 label = new XtraReport2();
            label.DataSource = dtoList;
            label.PrintingSystem.ShowMarginsWarning = false;
            label.PrintingSystem.ShowPrintStatusDialog = false;
            ReportPrintTool tool = new ReportPrintTool(label);
            tool.Print();
        }

        /// <summary>
        /// 更新物料信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            UpLocalMaterialInfo();
        }

        private void UpLocalMaterialInfo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SQLiteHelper.ConnectionString))
                {
                    LogHelper.WriteLog("打印标签界面，连接数据库字符为空！");
                }

                //Pivots.ConfigurationManager.Instance.LoadConfig();
                //string agencyname = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyname").InnerText;
                //string agencypassword = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencypassword").InnerText;
                //string agencyterritory = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyterritory").InnerText;
                //string agencysite = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencysite").InnerText;
                //string agencyport = Pivots.ConfigurationManager.Instance.ConfigDocument.SelectSingleNode("//agencyport").InnerText;
                //string agencyurl = "";
                //if (!string.IsNullOrWhiteSpace(agencysite) && !string.IsNullOrWhiteSpace(agencyport))
                //    agencyurl = agencysite + ":" + agencyport;
                //Pivots.ConfigurationManager.Instance.LoadConfig();

                string url = System.Configuration.ConfigurationManager.AppSettings["UpLabelBuilderURL"];
                string methodname = System.Configuration.ConfigurationManager.AppSettings["MethodName"];
                string password = System.Configuration.ConfigurationManager.AppSettings["Password"];

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("name", "");
                parameter.Add("_password", password);
                parameter.Add("code", "");
                parameter.Add("role", "");
                string ReturnVal = WebServiceHelper.HttpPostWebService(url, methodname, parameter, Encoding.GetEncoding("UTF-8"));

                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReturnVal);

                if (dictionary.ContainsKey("Status"))
                {
                    if (Convert.ToBoolean(dictionary["Status"].Trim()))
                    {
                        string json = dictionary["Data"].Trim();
                        if (string.IsNullOrWhiteSpace(json))
                            MessageBox.Show("Update data is empty!");
                        else
                        {
                            try
                            {
                                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                                string delsql = "delete from material";
                                new SQLiteHelper().ExecuteNonQuery(delsql);

                                #region 拼接addMaterialsql
                                StringBuilder sb = new StringBuilder();
                                sb.Append("insert into Material(");
                                sb.Append("Name,Code,Spec,Customer,MaterialClass,MinInventory,OptimisticLockField");
                                sb.Append(",Warehouse,Name2,DefaultPKG,Alias,IsForceBySpec,MLength,MWidth,MHeight,MWeight,IsNeedQA,SNP,SupplierName,SupplierCode,PackageModel,Param");
                                sb.Append(")");
                                sb.Append(" values (");
                                sb.Append("@Name,@Code,@Spec,@Customer,@MaterialClass,@MinInventory,@OptimisticLockField");
                                sb.Append(",@Warehouse,@Name2,@DefaultPKG,@Alias,@IsForceBySpec,@MLength,@MWidth,@MHeight,@MWeight,@IsNeedQA,@SNP,@SupplierName,@SupplierCode");
                                sb.Append(",@PackageModel,@Param)");
                                string sqladd = sb.ToString();
                                List<SQLiteParameter[]> list = new List<SQLiteParameter[]>();
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    SQLiteParameter[] parameters = {
                                            SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String,dt.Rows[i]["Name"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Code", DbType.String,dt.Rows[i]["Code"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Spec", DbType.String,dt.Rows[i]["Spec"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Customer", DbType.String,dt.Rows[i]["Customer"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MaterialClass", DbType.String,dt.Rows[i]["MaterialClass"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MinInventory", DbType.String,dt.Rows[i]["MinInventory"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@OptimisticLockField", DbType.String,dt.Rows[i]["OptimisticLockField"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Warehouse", DbType.String,dt.Rows[i]["Warehouse"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Name2", DbType.String,dt.Rows[i]["Name2"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@DefaultPKG", DbType.String,dt.Rows[i]["DefaultPKG"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Alias", DbType.String,dt.Rows[i]["Alias"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@IsForceBySpec", DbType.String,dt.Rows[i]["IsForceBySpec"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MLength", DbType.String,dt.Rows[i]["MLength"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MWidth", DbType.String,dt.Rows[i]["MWidth"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MHeight", DbType.String,dt.Rows[i]["MHeight"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@MWeight", DbType.String,dt.Rows[i]["MWeight"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@IsNeedQA", DbType.String,dt.Rows[i]["IsNeedQA"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@SNP", DbType.String,dt.Rows[i]["SNP"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@SupplierName", DbType.String,dt.Rows[i]["SupplierName"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@SupplierCode", DbType.String,dt.Rows[i]["SupplierID"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@PackageModel", DbType.String,dt.Rows[i]["PackageModel"].ToString().Trim()),
                                            SQLiteHelper.MakeSQLiteParameter("@Param", DbType.String,dt.Rows[i]["Param"].ToString().Trim())
                                        };
                                    list.Add(parameters);
                                }
                                #endregion

                                new SQLiteHelper().ExecuteNonQueryBatch(sqladd, list);

                                MessageBox.Show("Update data succeed!");
                            }
                            catch (Exception ex)
                            {
                                LogHelper.WriteLog(ex.Message);
                                throw ex;
                            }
                            finally
                            {
                                this.Cursor = Cursors.Default;//正常
                                GC.Collect();
                            }
                        }
                    }
                    else
                    {
                        string message = dictionary["Message"].Trim();
                        LogHelper.WriteLog(string.Format("请求异常，{0}", message));
                        MessageBox.Show(string.Format("Request exception, {0}", message));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                throw ex;
            }
            finally
            {
                this.Cursor = Cursors.Default;//正常
                GC.Collect();
            }
        }

    }
}
