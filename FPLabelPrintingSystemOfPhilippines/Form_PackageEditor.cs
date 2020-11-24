using FPLabelPrintingSystemOfPhilippines.ColumnNameConfig;
using FPLabelPrintingSystemOfPhilippines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FPLabelData;
using LabelPrintDAL;

namespace FPLabelPrintingSystemOfPhilippines
{
    public partial class Form_PackageEditor : Form
    {
        static object lockObj = new object();
        private PackageSet _package = null;
        public Form_PackageEditor(PackageSet package)
        {
            InitializeComponent();
            _package = package;
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
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PackageSet package = new PackageSet();
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("成品种类不可为空", "Warning");
                return;
            }
            #region 装载Package
            int oid = 0;
            package.Oid = int.TryParse(textBox2.Text.Trim(), out oid) ? oid : 0;
            package.FinishedProductVariety = textBox1.Text.Trim();
            package.A = checkBox1.Checked;
            package.B = checkBox2.Checked;
            package.C = checkBox3.Checked;
            package.D = checkBox4.Checked;
            package.E = checkBox5.Checked;
            package.F = checkBox5.Checked;
            package.G = checkBox5.Checked;
            package.H = checkBox5.Checked;
            package.I = checkBox5.Checked;
            package.J = checkBox5.Checked;
            package.K = checkBox5.Checked;
            package.L = checkBox5.Checked;
            package.M = checkBox5.Checked;
            package.HOME = checkBox6.Checked;
            package.SME = checkBox7.Checked;
            #endregion
            Task.Run(() => InsertOrUpdatePackageSet(package));
        }

        delegate void InsertOrUpdatePackageSetCallBackDel();
        /// <summary>
        /// 添加/更新包装配置
        /// </summary>
        /// <param name="package"></param>
        public void InsertOrUpdatePackageSet(PackageSet package)
        {
            lock (lockObj)
            {
                try
                {
                    if (package == null)
                    {
                        MessageBox.Show("待添加/更新的记录为空", "Error");
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,package.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@FinishedProductVariety", DbType.String,package.FinishedProductVariety),
                    SQLiteHelper.MakeSQLiteParameter("@A", DbType.Boolean,package.A),
                    SQLiteHelper.MakeSQLiteParameter("@B", DbType.Boolean,package.B),
                    SQLiteHelper.MakeSQLiteParameter("@C", DbType.Boolean,package.C),
                    SQLiteHelper.MakeSQLiteParameter("@D", DbType.Boolean,package.D),
                    SQLiteHelper.MakeSQLiteParameter("@E", DbType.Boolean,package.E),
                    SQLiteHelper.MakeSQLiteParameter("@F", DbType.Boolean,package.F),
                    SQLiteHelper.MakeSQLiteParameter("@G", DbType.Boolean,package.G),
                    SQLiteHelper.MakeSQLiteParameter("@H", DbType.Boolean,package.H),
                    SQLiteHelper.MakeSQLiteParameter("@I", DbType.Boolean,package.I),
                    SQLiteHelper.MakeSQLiteParameter("@J", DbType.Boolean,package.J),
                    SQLiteHelper.MakeSQLiteParameter("@K", DbType.Boolean,package.K),
                    SQLiteHelper.MakeSQLiteParameter("@L", DbType.Boolean,package.L),
                    SQLiteHelper.MakeSQLiteParameter("@M", DbType.Boolean,package.M),
                    SQLiteHelper.MakeSQLiteParameter("@HOME", DbType.Boolean,package.HOME),
                    SQLiteHelper.MakeSQLiteParameter("@SME", DbType.Boolean,package.SME)
                    };
                    paramList.Add(parameter);
                    if (package.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into PackageSet (FinishedProductVariety,A,B,C,D,E,HOME,SME) ")
                            .Append(@"values ( ")
                            .Append(@"@FinishedProductVariety,@A,@B,@C,@D,@E,@HOME,@SME ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update PackageSet set FinishedProductVariety=@FinishedProductVariety,A=@A,B=@B,C=@C,D=@D,E=@E,HOME=@HOME,SME=@SME ")
                            .Append(@" WHERE Oid=@Oid");
                    }
                    new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    InsertOrUpdatePackageSetCallBackDel del = InsertOrUpdatePackageSetCallBack;
                    this.BeginInvoke(del);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("InsertOrUpdatePackageSet", ex);
                }
            }
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        public void InsertOrUpdatePackageSetCallBack()
        {
            this.Close();
            //刷新列表记录
            Form1 preForm = Application.OpenForms["Form1"] as Form1;
            Task.Run(() => preForm.QueryPackage());
        }

        private void Form_PackageEditor_Load(object sender, EventArgs e)
        {
            if (_package == null || _package.Oid == 0)
                return;
            #region 显示待更新的记录
            textBox2.Text = _package.Oid.ToString();
            textBox1.Text = _package.FinishedProductVariety;
            checkBox1.Checked = _package.A;
            checkBox2.Checked = _package.B;
            checkBox3.Checked = _package.C;
            checkBox4.Checked = _package.D;
            checkBox5.Checked = _package.E;
            checkBox5.Checked = _package.F;
            checkBox5.Checked = _package.G;
            checkBox5.Checked = _package.H;
            checkBox5.Checked = _package.I;
            checkBox5.Checked = _package.J;
            checkBox5.Checked = _package.K;
            checkBox5.Checked = _package.L;
            checkBox5.Checked = _package.M;
            checkBox6.Checked = _package.HOME;
            checkBox7.Checked = _package.SME;
            #endregion
        }
    }
}
