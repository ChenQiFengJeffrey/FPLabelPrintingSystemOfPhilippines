using FPLabelData.Entity;
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

namespace ExtractInventoryTool.EditorForm
{
    public partial class Form_MaterialEditor : Form
    {
        #region 私有属性
        private ExtractInventoryTool_Material _material = null;
        private List<ExtractInventoryTool_Client> _clientList = null;
        #endregion
        public Form_MaterialEditor(ExtractInventoryTool_Material material)
        {
            InitializeComponent();
            _material = material;
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

        private void Form_MaterialEditor_Load(object sender, EventArgs e)
        {
            #region 显示待更新的记录
            if (_material != null && _material.Oid != 0)
            {
                textBox1.Text = _material.Oid.ToString();
                textBox2.Text = _material.Name;
                textBox3.Text = _material.Code;
                textBox4.Text = _material.Supplier;
                textBox5.Text = _material.SupplierCode;
            }
            #endregion
            #region 加载客户配置
            Task.Run(() => QueryClient());
            #endregion
            return;
        }

        #region 加载客户配置
        delegate void QueryClientCallbackDel(List<ExtractInventoryTool_Client> clientList);
        private void QueryClient()
        {
                try
                {
                    DataTable dt = new ExtractInventoryTool_ClientBLL().QueryClient();
                    _clientList = new List<ExtractInventoryTool_Client>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ExtractInventoryTool_Client client = new ExtractInventoryTool_Client();
                        client.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                        client.Name = row["Name"] == DBNull.Value ? "" : (string)row["Name"];
                        client.UniqueCode = row["UniqueCode"] == DBNull.Value ? "" : (string)row["UniqueCode"];
                        _clientList.Add(client);
                    }
                    QueryClientCallbackDel del = QueryClientCallback;
                    comboBox1.BeginInvoke(del, _clientList);
                    return;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("QueryClient", ex);
                }

        }
        private void QueryClientCallback(List<ExtractInventoryTool_Client> clientList)
        {
            comboBox1.DataSource = clientList;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Oid";
            comboBox1.SelectedItem = null;
            if (_material != null)
            {
                ExtractInventoryTool_Client selectedPrint = clientList.FirstOrDefault(p => p.Oid == _material.Client);
                if (selectedPrint != null)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(selectedPrint);
                }
            }
            return;
        }

        #endregion

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            #region 判空处理
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("名称不能为空", "Warning");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("代码不能为空", "Warning");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("供应商名称不能为空", "Warning");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("供应商代码不能为空", "Warning");
                return;
            }
            #endregion

            #region 装载Ro
            ExtractInventoryTool_Material material = new ExtractInventoryTool_Material();
            int oid = 0;
            material.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            int clientOid = 0;
            material.Client = int.TryParse(comboBox1.SelectedValue.ToString(), out clientOid) ? clientOid : 0;
            material.Name = textBox2.Text.Trim();
            material.Code = textBox3.Text.Trim();
            material.Supplier = textBox4.Text.Trim();
            material.SupplierCode = textBox5.Text.Trim();
            material.UniqueCode = "c" + _clientList.First(c => c.Oid == material.Client).UniqueCode + "m" + material.Code + "s" + material.SupplierCode;
            #endregion
            Task.Run(() => InsertOrUpdateMaterial(material));
            return;
        }

        delegate void InsertOrUpdateMaterialCallBackDel(string errorMessage);
        /// <summary>
        /// 添加/更新物料配置
        /// </summary>
        /// <param name="material"></param>
        private void InsertOrUpdateMaterial(ExtractInventoryTool_Material material)
        {
            try
            {
                string errorMessage = string.Empty;
                new ExtractInventoryTool_MaterialBLL().InsertOrUpdateMaterial(material, out errorMessage);
                InsertOrUpdateMaterialCallBackDel del = InsertOrUpdateMaterialCallBack;
                this.BeginInvoke(del,errorMessage);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void InsertOrUpdateMaterialCallBack(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        /// <summary>
        /// 窗体激活时初始化焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_MaterialEditor_Activated(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
    }
}
