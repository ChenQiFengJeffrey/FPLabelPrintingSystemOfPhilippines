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
    public partial class Form_GoodEditor : Form
    {
        static object lockObj = new object();
        private GoodSet _good = null;
        public Form_GoodEditor(GoodSet good)
        {
            InitializeComponent();
            _good = good;
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
        private void Form_GoodEditor_Load(object sender, EventArgs e)
        {
            #region 显示待更新的记录
            if (_good != null && _good.Oid != 0)
            {
                textBox1.Text = _good.Oid.ToString();
                textBox3.Text = _good.MaterialNum;
                textBox2.Text = _good.MaterialName;
                numericUpDown1.Value = _good.QTY;
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
            if (_good != null && _good.Oid != 0)
            {
                PrintSet selectedPrint = printlist.FirstOrDefault(p => p.Oid == _good.FinishedProductNum);
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
                MessageBox.Show("原材料号不可为空", "Warning");
                return;
            }
            #region 装载Good
            GoodSet good = new GoodSet();
            int oid = 0;
            good.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            int fpoid = 0;
            good.FinishedProductNum = int.TryParse(comboBox1.SelectedValue.ToString(), out fpoid) ? fpoid : 0;
            good.MaterialNum = textBox3.Text.Trim();
            good.MaterialName = textBox2.Text.Trim();
            good.QTY = Convert.ToInt32(numericUpDown1.Value);
            #endregion
            //InsertOrUpdateGoodSet(good);
            Task.Run(() => InsertOrUpdateGoodSet(good));
            return;
        }
        delegate void InsertOrUpdateGoodSetCallBackDel();
        /// <summary>
        /// 添加/更新成品配置
        /// </summary>
        /// <param name="good"></param>
        private void InsertOrUpdateGoodSet(GoodSet good)
        {
            lock (lockObj)
            {
                try
                {
                    if (good == null)
                    {
                        MessageBox.Show("待添加/更新的记录为空", "Error");
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,good.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.Int32,good.FinishedProductNum),
                    SQLiteHelper.MakeSQLiteParameter("@MaterialNum", DbType.String,good.MaterialNum),
                    SQLiteHelper.MakeSQLiteParameter("@MaterialName", DbType.String,good.MaterialName),
                    SQLiteHelper.MakeSQLiteParameter("@QTY", DbType.Int32,good.QTY)
                    };
                    paramList.Add(parameter);
                    if (good.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into GoodSet (FinishedProductNum,MaterialNum,MaterialName,QTY) ")
                            .Append(@"values ( ")
                            .Append(@"@FinishedProductNum,@MaterialNum,@MaterialName,@QTY ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update GoodSet set FinishedProductNum=@FinishedProductNum,MaterialNum=@MaterialNum,MaterialName=@MaterialName,QTY=@QTY ")
                            .Append(@" WHERE Oid=@Oid");
                    }
                    new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    InsertOrUpdateGoodSetCallBackDel del = InsertOrUpdateGoodSetCallBack;
                    this.BeginInvoke(del);
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private void InsertOrUpdateGoodSetCallBack()
        {
            this.Close();
            //刷新列表记录
            Form1 preForm = Application.OpenForms["Form1"] as Form1;
            Task.Run(() => preForm.QueryGoodSet());
            return;
        }
    }
}
