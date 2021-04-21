using ExtractInventoryTool.TabForm;
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
    public partial class Form_ClientEditor : Form
    {
        #region 私有属性
        static object lockObj = new object();
        private ExtractInventoryTool_Client _client = null;
        #endregion
        public Form_ClientEditor(ExtractInventoryTool_Client client)
        {
            InitializeComponent();
            _client = client;
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

        private void ClientEditor_Load(object sender, EventArgs e)
        {
            #region 显示待更新的记录
            if (_client != null && _client.Oid != 0)
            {
                textBox1.Text = _client.Oid.ToString();
                textBox2.Text = _client.Name;
                textBox3.Text = _client.UniqueCode;
                textBox4.Text = _client.RegexRule;
                richTextBox1.Text = _client.Remark;
            }
            #endregion
            return;
        }

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
            #endregion
            #region 装载client
            ExtractInventoryTool_Client client = new ExtractInventoryTool_Client();
            int oid = 0;
            client.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            client.Name = textBox2.Text.Trim();
            client.UniqueCode = textBox3.Text.Trim();
            client.RegexRule = textBox4.Text.Trim();
            client.Remark = richTextBox1.Text.Trim();
            #endregion
            Task.Run(() => InsertOrUpdateClient(client));
            return;
        }


        delegate void InsertOrUpdateClientCallBackDel(string errorMessage);
        /// <summary>
        /// 添加/更新成品Ro#配置
        /// </summary>
        /// <param name="client"></param>
        private void InsertOrUpdateClient(ExtractInventoryTool_Client client)
        {
            string errorMessage = string.Empty;
            new ExtractInventoryTool_ClientBLL().InsertOrUpdateClient(client,out errorMessage);
            InsertOrUpdateClientCallBackDel del = InsertOrUpdateClientCallBack;
            this.BeginInvoke(del,errorMessage);
            return;
        }
        private void InsertOrUpdateClientCallBack(string errorMessage)
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
        private void Form_ClientEditor_Activated(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
    }
}
