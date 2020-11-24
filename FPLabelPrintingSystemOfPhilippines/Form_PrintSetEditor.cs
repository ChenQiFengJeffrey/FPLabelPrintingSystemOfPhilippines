using FPLabelData;
using FPLabelPrintingSystemOfPhilippines.ColumnNameConfig;
using LabelPrintDAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPLabelPrintingSystemOfPhilippines
{
    public partial class Form_PrintSetEditor : Form
    {
        static object lockObj = new object();
        private PrintSet _print = null;
        private DataTable _snTable = null;
        private DataTable _packageTable = null;
        private List<Material> _materialList = null;
        public Form_PrintSetEditor(PrintSet print)
        {
            InitializeComponent();
            _print = print;
            InitSNTable();
            InitPackageTable();
        }
        /// <summary>
        /// 初始化SN
        /// </summary>
        public void InitSNTable()
        {
            SNConfig sn = new SNConfig();
            _snTable = new DataTable();
            _snTable.Columns.Add(sn.Oid, typeof(int));
            _snTable.Columns.Add(sn.Name, typeof(string));
            _snTable.Columns.Add(sn.IsUsed, typeof(bool));
            _snTable.Columns.Add(sn.Order, typeof(int));
            _snTable.Columns.Add(sn.Code, typeof(string));
            bindGrid(dataGridView1, _snTable, new int[] { 0 });
        }
        /// <summary>
        /// 初始化PackageConfig
        /// </summary>
        public void InitPackageTable()
        {
            PackageConfig package = new PackageConfig();
            _packageTable = new DataTable();
            _packageTable.Columns.Add(package.Oid, typeof(int));
            _packageTable.Columns.Add(package.Name, typeof(string));
            _packageTable.Columns.Add(package.IsChecked, typeof(bool));
            _packageTable.Columns.Add(package.Order, typeof(int));
            _packageTable.Columns.Add(package.Code, typeof(string));
            bindGrid(dataGridView2, _packageTable, new int[] { 0 });
        }
        /// <summary>
        /// 绑定数据源到GridView
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="dt"></param>
        /// <param name="idAry"></param>
        public void bindGrid(DataGridView dv, DataTable dt, int[] idAry = null)
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

        private void Form_PrintSetEditor_Load(object sender, EventArgs e)
        {
            #region 加载物料信息
            Task.Run(() => QueryMaterial());
            #endregion
            if (_print == null || _print.Oid == 0)
                return;
            #region 显示待更新的记录
            textBox1.Text = _print.Oid.ToString();
            textBox4.Text = _print.CreateTime.Equals(DateTime.MinValue) ? "" : _print.CreateTime.ToString();
            textBox2.Text = _print.FinishedProductNum;
            SN_JsonToTable(_print.SN);
            Package_JsonToTable(_print.Package);
            #endregion
        }

        delegate void QueryMaterialCallbackDel(List<Material> printList);
        private void QueryMaterial()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from Material order by OID desc");
                    //_packageSetTable.Rows.Clear();//清除行数据，但是行保留
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                    List<Material> materialList = new List<Material>();
                    foreach (DataRow row in dt.Rows)
                    {
                        Material print = new Material();
                        print.OID = row["OID"] == DBNull.Value ? 0 : Convert.ToInt32(row["OID"]);
                        print.Name = row["Name"] == DBNull.Value ? "" : (string)row["Name"];
                        print.Code = row["Code"] == DBNull.Value ? "" : (string)row["Code"];
                        materialList.Add(print);
                    }
                    _materialList = materialList;
                    QueryMaterialCallbackDel del = QueryMaterialCallback;
                    comboBox1.BeginInvoke(del, materialList);
                    return;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("QueryPrint", ex);
                }
            }
        }
        private void QueryMaterialCallback(List<Material> materialList)
        {
            comboBox1.DataSource = materialList;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Name";
            if (_print != null && _print.Oid != 0)
            {
                Material selectedPrint = materialList.FirstOrDefault(p => p.Name == _print.FinishedProductNum);
                comboBox1.Text = selectedPrint == null ? "" : selectedPrint.Name;
            }
            return;
        }

        /// <summary>
        /// SN_Json转换为Tabel
        /// </summary>
        /// <param name="snJsonnString"></param>
        public void SN_JsonToTable(string snJsonnString)
        {
            if (string.IsNullOrWhiteSpace(snJsonnString))
                return;
            List<SN> snList = JsonConvert.DeserializeObject<List<SN>>(snJsonnString);
            if (snList == null || snList.Count == 0)
                return;
            _snTable.Clear();
            SNConfig config = new SNConfig();
            foreach (SN item in snList)
            {
                DataRow row = _snTable.NewRow();
                row[config.Oid] = item.Oid;
                row[config.Name] = item.Name;
                row[config.IsUsed] = item.IsUsed;
                row[config.Order] = item.Order;
                row[config.Code] = item.Code;
                _snTable.Rows.Add(row);
            }
            bindGrid(dataGridView1, _snTable, new int[] { 0 });
        }
        /// <summary>
        /// SN_Table转换为JSON
        /// </summary>
        /// <returns></returns>
        public string SN_TableToJson()
        {
            string result = string.Empty;
            if (dataGridView1.DataSource == null)
            {
                return result;
            }
            DataTable dt = (DataTable)dataGridView1.DataSource;
            if (dt == null || dt.Rows.Count <= 0)
            {
                return result;
            }
            SNConfig config = new SNConfig();
            List<SN> snList = new List<SN>();
            foreach (DataRow row in dt.Rows)
            {
                SN sn = new SN();
                sn.Oid = row[config.Oid] == DBNull.Value ? 0 : Convert.ToInt32(row[config.Oid]);
                sn.Name = row[config.Name] == DBNull.Value ? "" : (string)row[config.Name];
                sn.IsUsed = row[config.IsUsed] == DBNull.Value ? false : Convert.ToBoolean(row[config.IsUsed]);
                sn.Order = row[config.Order] == DBNull.Value ? 0 : Convert.ToInt32(row[config.Order]);
                sn.Code = row[config.Code] == DBNull.Value ? "" : (string)row[config.Code];
                snList.Add(sn);
            }
            result = JsonConvert.SerializeObject(snList);
            return result;
        }

        /// <summary>
        /// Package_Json转换为Table
        /// </summary>
        /// <param name="packageJsonnString"></param>
        public void Package_JsonToTable(string packageJsonnString)
        {
            if (string.IsNullOrWhiteSpace(packageJsonnString))
                return;
            List<Package> packageList = JsonConvert.DeserializeObject<List<Package>>(packageJsonnString);
            if (packageList == null || packageList.Count == 0)
                return;
            _packageTable.Clear();
            PackageConfig config = new PackageConfig();
            foreach (Package item in packageList)
            {
                DataRow row = _packageTable.NewRow();
                row[config.Oid] = item.Oid;
                row[config.Name] = item.Name;
                row[config.IsChecked] = item.IsChecked;
                row[config.Order] = item.Order;
                row[config.Code] = item.Code;
                _packageTable.Rows.Add(row);
            }
            bindGrid(dataGridView2, _packageTable, new int[] { 0 });
        }
        /// <summary>
        /// Package_Table转换为JSON
        /// </summary>
        /// <returns></returns>
        public string Package_TableToJson()
        {
            string result = string.Empty;
            if (dataGridView2.DataSource == null)
            {
                return result;
            }
            DataTable dt = (DataTable)dataGridView2.DataSource;
            if (dt == null || dt.Rows.Count <= 0)
            {
                return result;
            }
            PackageConfig config = new PackageConfig();
            List<Package> packList = new List<Package>();
            foreach (DataRow row in dt.Rows)
            {
                Package package = new Package();
                package.Oid = row[config.Oid] == DBNull.Value ? 0 : Convert.ToInt32(row[config.Oid]);
                package.Name = row[config.Name] == DBNull.Value ? "" : (string)row[config.Name];
                package.IsChecked = row[config.IsChecked] == DBNull.Value ? false : Convert.ToBoolean(row[config.IsChecked]);
                package.Order = row[config.Order] == DBNull.Value ? 0 : Convert.ToInt32(row[config.Order]);
                package.Code = row[config.Code] == DBNull.Value ? "" : (string)row[config.Code];
                packList.Add(package);
            }
            result = JsonConvert.SerializeObject(packList);
            return result;
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("成品料号不可为空", "Warning");
                return;
            }
            #region 装载Print
            PrintSet print = new PrintSet();
            int oid = 0;
            print.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            print.FinishedProductNum = textBox2.Text.Trim();
            print.SN = SN_TableToJson();
            print.Package = Package_TableToJson();
            #endregion
            #region 判断SN和Package
            if (!string.IsNullOrWhiteSpace(print.SN))
            {
                List<SN> snList = JsonConvert.DeserializeObject<List<SN>>(print.SN);
                if (snList.Where(s => s.IsUsed == true).Select(s => s.Name.Trim().ToUpper()).Distinct().Count() != snList.Where(s => s.IsUsed == true).ToList().Count)
                {
                    MessageBox.Show("The name of SN is unique!");
                }
            }
            if (!string.IsNullOrWhiteSpace(print.Package))
            {
                List<Package> packageList = JsonConvert.DeserializeObject<List<Package>>(print.Package);
                if (packageList.Select(s => s.Name.Trim().ToUpper()).Distinct().Count() != packageList.Count)
                {
                    MessageBox.Show("The name of Package is unique!", "Warning");
                }
            }
            #endregion
            Task.Run(() => InsertOrUpdatePrintSet(print));
            //InsertOrUpdatePrintSet(print);
            return;
        }
        delegate void InsertOrUpdatePrintSetCallBackDel(bool isrepeat);
        /// <summary>
        /// 添加/更新成品配置
        /// </summary>
        /// <param name="print"></param>
        private void InsertOrUpdatePrintSet(PrintSet print)
        {
            lock (lockObj)
            {
                try
                {
                    if (print == null)
                    {
                        MessageBox.Show("待添加/更新的记录为空", "Error");
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    StringBuilder queryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,print.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.String,print.FinishedProductNum),
                    SQLiteHelper.MakeSQLiteParameter("@SN", DbType.String,print.SN),
                    SQLiteHelper.MakeSQLiteParameter("@Package", DbType.String,print.Package)
                    };
                    paramList.Add(parameter);
                    if (print.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into PrintSet (FinishedProductNum,SN,Package) ")
                            .Append(@"values ( ")
                            .Append(@"@FinishedProductNum,@SN,@Package ")
                            .Append(@")");
                        queryStrbd.Append(@"select * from PrintSet where FinishedProductNum=@FinishedProductNum");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update PrintSet set FinishedProductNum=@FinishedProductNum,SN=@SN,Package=@Package ")
                            .Append(@" WHERE Oid=@Oid");
                        queryStrbd.Append(@"select * from PrintSet where FinishedProductNum=@FinishedProductNum and Oid!=@Oid");
                    }
                    DataTable existTable = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString(), parameter);
                    bool isrepeat = true;
                    if (existTable.Rows.Count == 0)
                    {
                        isrepeat = false;
                        new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    }
                    InsertOrUpdatePrintSetCallBackDel del = InsertOrUpdatePrintSetCallBack;
                    this.BeginInvoke(del, isrepeat);
                    return;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("InsertOrUpdatePrintSet", ex);
                }
            }
        }
        private void InsertOrUpdatePrintSetCallBack(bool isrepeat)
        {
            if (isrepeat)
            {
                MessageBox.Show("The FinishedProductNum is unique!", "Warning");
                return;
            }
            this.Close();
            //刷新列表记录
            Form1 preForm = Application.OpenForms["Form1"] as Form1;
            Task.Run(() => preForm.QueryPrintSet());
            return;
        }
        /// <summary>
        /// 添加SN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt1 = dataGridView1.DataSource as DataTable;
            dt1.Rows.Add(dt1.NewRow());
        }
        /// <summary>
        /// 删除SN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in rowCollection)
            {
                dataGridView1.Rows.Remove(row);
            }
        }
        /// <summary>
        /// 添加Package
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt2 = dataGridView2.DataSource as DataTable;
            dt2.Rows.Add(dt2.NewRow());
        }
        /// <summary>
        /// 删除Package
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView2.SelectedRows;
            foreach (DataGridViewRow row in rowCollection)
            {
                dataGridView2.Rows.Remove(row);
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text.Trim().ToUpper();
            comboBox1.DataSource = _materialList.Where(m => m.Name.ToUpper().Contains(text)).ToList();
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string text = cb.Text.Trim().ToUpper();
            int startIndex = cb.SelectionStart;
            comboBox1.DataSource = _materialList.Where(m => m.Name.ToUpper().Contains(text)).ToList();
            comboBox1.Text = text;
            comboBox1.Select(startIndex, 0);
            return;
        }
    }
}
