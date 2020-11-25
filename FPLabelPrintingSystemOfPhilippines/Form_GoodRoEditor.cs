using FPLabelData;
using FPLabelPrintingSystemOfPhilippines.ColumnNameConfig;
using LabelPrintDAL;
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
    public partial class Form_GoodRoEditor : Form
    {
        static object lockObj = new object();
        private RoSet _goodro = null;
        public Form_GoodRoEditor(RoSet goodro)
        {
            InitializeComponent();
            _goodro = goodro;
        }
        /// <summary>
        /// 窗体取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 加载Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_GoodRoEditor_Load(object sender, EventArgs e)
        {
            #region 显示待更新的记录
            if (_goodro != null && _goodro.Oid != 0)
            {
                textBox1.Text = _goodro.Oid.ToString();
                textBox3.Text = _goodro.RoNumber;
            }
            #endregion
            #region 加载包装配置
            Task.Run(() => QueryPrint());
            //QueryPackage();
            #endregion
            return;
        }
        delegate void QueryPrintCallbackDel(List<PrintSet> printList);
        private void QueryPrint()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from PrintSet order by Oid desc");
                    //_packageSetTable.Rows.Clear();//清除行数据，但是行保留
                    DataTable dt = new DataTable();
                    dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                    List<PrintSet> packageList = new List<PrintSet>();
                    foreach (DataRow row in dt.Rows)
                    {
                        PrintSet print = new PrintSet();
                        print.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                        print.FinishedProductNum = row["FinishedProductNum"] == DBNull.Value ? "" : (string)row["FinishedProductNum"];
                        packageList.Add(print);
                    }
                    QueryPrintCallbackDel del = QueryPrintCallback;
                    comboBox1.BeginInvoke(del, packageList);
                    return;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("QueryPrint", ex);
                }
            }
        }
        private void QueryPrintCallback(List<PrintSet> printlist)
        {
            comboBox1.DataSource = printlist;
            comboBox1.DisplayMember = "FinishedProductNum";
            comboBox1.ValueMember = "Oid";
            if (_goodro != null && _goodro.Oid != 0)
            {
                PrintSet selectedPrint = printlist.FirstOrDefault(p => p.Oid == _goodro.FinishedProductNum);
                comboBox1.Text = selectedPrint == null ? "" : selectedPrint.FinishedProductNum;
            }
            return;
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("RoNumber is null", "Warning");
                return;
            }
            #region 装载Ro
            RoSet goodro = new RoSet();
            int oid = 0;
            goodro.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            int fpoid = 0;
            goodro.FinishedProductNum = int.TryParse(comboBox1.SelectedValue.ToString(), out fpoid) ? fpoid : 0;
            goodro.RoNumber = textBox3.Text.Trim();
            #endregion
            Task.Run(() => InsertOrUpdateGoodRoSet(goodro));
            return;
        }
        delegate void InsertOrUpdateRoSetCallBackDel();
        /// <summary>
        /// 添加/更新成品Ro#配置
        /// </summary>
        /// <param name="ro"></param>
        private void InsertOrUpdateGoodRoSet(RoSet ro)
        {
            lock (lockObj)
            {
                try
                {
                    if (ro == null)
                    {
                        MessageBox.Show("RoSet is null!", "Error");
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,ro.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.Int32,ro.FinishedProductNum),
                    SQLiteHelper.MakeSQLiteParameter("@RoNumber", DbType.String,ro.RoNumber),
                    };
                    paramList.Add(parameter);
                    if (ro.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into RoSet (FinishedProductNum,RoNumber) ")
                            .Append(@"values ( ")
                            .Append(@"@FinishedProductNum,@RoNumber ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update RoSet set FinishedProductNum=@FinishedProductNum,RoNumber=@RoNumber ")
                            .Append(@" WHERE Oid=@Oid");
                    }
                    new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    InsertOrUpdateRoSetCallBackDel del = InsertOrUpdateRoSetCallBack;
                    this.BeginInvoke(del);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private void InsertOrUpdateRoSetCallBack()
        {
            this.Close();
            //刷新列表记录
            Form1 preForm = Application.OpenForms["Form1"] as Form1;
            Task.Run(() => preForm.QueryGoodRoSet());
            return;
        }
    }
}
