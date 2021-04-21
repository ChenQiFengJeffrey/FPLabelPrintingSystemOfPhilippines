using ExtractInventoryTool.ToolForm;
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
    public partial class Form_BOMUploader : Form
    {
        #region 私有属性
        private List<ExtractInventoryTool_BOM> _excelData = null;
        #endregion
        public Form_BOMUploader()
        {
            InitializeComponent();
        }

        private void Form_BOMUploader_Load(object sender, EventArgs e)
        {
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
                List<ExtractInventoryTool_Client> clientList = new List<ExtractInventoryTool_Client>();
                foreach (DataRow row in dt.Rows)
                {
                    ExtractInventoryTool_Client client = new ExtractInventoryTool_Client();
                    client.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                    client.Name = row["Name"] == DBNull.Value ? "" : (string)row["Name"];
                    client.UniqueCode = row["UniqueCode"] == DBNull.Value ? "" : (string)row["UniqueCode"];
                    client.RegexRule = row["RegexRule"] == DBNull.Value ? "" : (string)row["RegexRule"];
                    clientList.Add(client);
                }
                QueryClientCallbackDel del = QueryClientCallback;
                comboBox1.BeginInvoke(del, clientList);
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
            return;
        }
        #endregion

        /// <summary>
        /// 上传BOM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            #region 勾选客户
            ExtractInventoryTool_Client selectedPrint = (ExtractInventoryTool_Client)comboBox1.SelectedItem;
            if (selectedPrint == null)
            {
                MessageBox.Show("请选中一条客户进行导入BOM", "Warning");
                return;
            }
            string regexRule = selectedPrint.RegexRule;
            #endregion
            #region 导入Excel
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.Cancel) { return; }
            textBox1.Text = openFile.FileName;
            LoadingHelper.ShowLoadingScreen();
            Task.Run(() => ImportBOMExcel(openFile.FileName, selectedPrint));
            //ImportBOMExcel(openFile.FileName, selectedPrint);
            #endregion
        }
        #region 导入BOM
        delegate void SaveBOMExcelCallbackDel(bool isSucceed, string errorMessage);
        public void SaveBOMExcelCallback(bool isSucceed, string errorMessage)
        {
            LoadingHelper.CloseForm();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (!isSucceed)
                {
                    MessageBox.Show(errorMessage, "Error");
                    this.DialogResult = DialogResult.None;
                    this.Close();
                    return;
                }
                DialogResult isDelete = MessageBox.Show(errorMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (isDelete != DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.None;
                    this.Close();
                    return;
                }
                LoadingHelper.ShowLoadingScreen();
                Task.Run(() => SaveBOMExcel(_excelData, true));
                return;
            }
            _excelData = null;
            MessageBox.Show("导入成功！", "Success");
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        delegate void ImportBOMExcelCallbackDel(List<ExtractInventoryTool_BOM> excelData, string errorMessage);
        public void ImportBOMExcelCallback(List<ExtractInventoryTool_BOM> excelData, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                LoadingHelper.CloseForm();
                MessageBox.Show(errorMessage, "Error");
                this.DialogResult = DialogResult.None;
                this.Close();
                return;
            }
            Task.Run(() => SaveBOMExcel(_excelData, false));
        }

        public void ImportBOMExcel(string fileName, ExtractInventoryTool_Client client)
        {
            _excelData = null;
            string errorMessage = string.Empty;
            _excelData = new NPOIHelper().ExcelToBOMList(fileName, client, out errorMessage);
            ImportBOMExcelCallbackDel del = ImportBOMExcelCallback;
            this.BeginInvoke(del, _excelData, errorMessage);
        }

        public void SaveBOMExcel(List<ExtractInventoryTool_BOM> excelData, bool isCover)
        {
            string errorMessage = string.Empty;
            bool importResult = false;
            importResult = new ExtractInventoryTool_BOMBLL().ImportBOM(excelData, isCover, out errorMessage);
            SaveBOMExcelCallbackDel del = SaveBOMExcelCallback;
            this.BeginInvoke(del, importResult, errorMessage);
        }
        #endregion
    }
}
