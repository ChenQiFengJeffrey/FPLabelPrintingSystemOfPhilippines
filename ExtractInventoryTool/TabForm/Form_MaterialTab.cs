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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractInventoryTool.TabForm
{
    public partial class Form_MaterialTab : Form
    {
        #region 私有属性
        private DataTable _clientTable = null;
        private DataTable _materialTable = null;
        private List<ExtractInventoryTool_Material> _excelData = null;
        #endregion
        public Form_MaterialTab()
        {
            InitializeComponent();
            InitClientTable(dataGridView1, new int[] { 0, 2, 3 });//物料备案客户配置
            InitMaterialTable(dataGridView2, new int[] { 0, 5 });//物料备案物料配置
        }

        #region 初始化grid
        /// <summary>
        /// 初始化客户grid
        /// </summary>
        public void InitClientTable(DataGridView dv, int[] idAry)
        {
            ClientConfig client = new ClientConfig();
            _clientTable = new DataTable();
            _clientTable.Columns.Add(client.Oid, typeof(string));
            _clientTable.Columns.Add(client.Name, typeof(string));
            _clientTable.Columns.Add(client.UniqueCode, typeof(string));
            _clientTable.Columns.Add(client.Remark, typeof(string));
            BindGrid(dv, _clientTable, idAry);
        }
        /// <summary>
        /// 初始化物料grid
        /// </summary>
        public void InitMaterialTable(DataGridView dv, int[] idAry)
        {
            MaterialConfig material = new MaterialConfig();
            _materialTable = new DataTable();
            _materialTable.Columns.Add(material.Oid, typeof(string));
            _materialTable.Columns.Add(material.Name, typeof(string));
            _materialTable.Columns.Add(material.Code, typeof(string));
            _materialTable.Columns.Add(material.Supplier, typeof(string));
            _materialTable.Columns.Add(material.SupplierCode, typeof(string));
            _materialTable.Columns.Add(material.Client, typeof(string));
            BindGrid(dv, _materialTable, idAry);
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
        /// 刷新客户事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => QueryClient());
            return;
        }
        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int clientOid = 0;
            ExtractInventoryTool_Material material = null;
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count != 1)
            {
                MessageBox.Show("请选中一条客户进行添加物料备案", "Warning");
                return;
            }
            row = rowCollection[0];
            if (row != null && row.Cells[0].Value != null)
            {
                material = new ExtractInventoryTool_Material();
                material.Oid = 0;
                material.Client = int.TryParse(row.Cells[0].Value.ToString().Trim(), out clientOid) ? clientOid : 0;
            }
            Form_MaterialEditor editor = new Form_MaterialEditor(material);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryMaterialByClientID(clientOid.ToString()));
            }
            return;
        }
        /// <summary>
        /// 更新物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            int clientOid = 0;
            ExtractInventoryTool_Material material = null;
            DataGridViewSelectedRowCollection clientRowCollection = dataGridView1.SelectedRows;
            DataGridViewSelectedRowCollection materialRowCollection = dataGridView2.SelectedRows;
            DataGridViewRow clientRow = null;
            DataGridViewRow materialRow = null;
            if (clientRowCollection.Count != 1)
            {
                MessageBox.Show("请选中一条客户进行更新物料备案", "Warning");
                return;
            }
            if (materialRowCollection.Count != 1)
            {
                MessageBox.Show("请选中一条物料进行更新", "Warning");
                return;
            }
            clientRow = clientRowCollection[0];
            materialRow = materialRowCollection[0];
            if (clientRow != null && clientRow.Cells[0].Value != null
                && materialRow != null && materialRow.Cells[0].Value != null)
            {
                material = new ExtractInventoryTool_Material();
                int oid = 0;
                material.Oid = int.TryParse(materialRow.Cells[0].Value.ToString().Trim(), out oid) ? oid : 0;
                material.Name = materialRow.Cells[1].Value.ToString().Trim();
                material.Code = materialRow.Cells[2].Value.ToString().Trim();
                material.Supplier = materialRow.Cells[3].Value.ToString().Trim();
                material.SupplierCode = materialRow.Cells[4].Value.ToString().Trim();
                material.Client = int.TryParse(clientRow.Cells[0].Value.ToString().Trim(), out clientOid) ? clientOid : 0;
            }
            Form_MaterialEditor editor = new Form_MaterialEditor(material);
            editor.ShowDialog(this);
            if (editor.DialogResult == DialogResult.OK)
            {
                Task.Run(() => QueryMaterialByClientID(clientOid.ToString()));
            }
            return;
        }
        /// <summary>
        /// 删除物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rowCollection = dataGridView2.SelectedRows;
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
            Task.Run(() => DeleteMaterialRecords("Material", ids));
        }
        /// <summary>
        /// 导入物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            #region 勾选客户
            DataGridViewSelectedRowCollection rowCollection = dataGridView1.SelectedRows;
            DataGridViewRow row = null;
            if (rowCollection.Count != 1)
            {
                MessageBox.Show("请选中一条客户进行添加物料备案", "Warning");
                return;
            }
            row = rowCollection[0];
            int clientOid = 0;
            clientOid = int.TryParse(row.Cells[0].Value.ToString().Trim(), out clientOid) ? clientOid : 0;
            string clientUniqueCode = row.Cells[2].Value.ToString().Trim();
            #endregion
            #region 导入Excel
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.Cancel) { return; }
            LoadingHelper.ShowLoadingScreen();
            Task.Run(() => ImportMaterialExcel(openFile.FileName, clientOid, clientUniqueCode));
            #endregion
        }
        /// <summary>
        /// 客户列表单元格选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            string choosedClientID= this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            Task.Run(() => QueryMaterialByClientID(choosedClientID));
        }

        #region 查询客户
        /// <summary>
        /// 查询客户回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryClientCallbackDel(DataTable dt);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryClientCallback(DataTable dt)
        {
            _clientTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _clientTable.Rows.Add(
                    row["Oid"],
                    row["Name"],
                    row["UniqueCode"],
                    row["Remark"]
                    );
            }
            BindGrid(dataGridView1, _clientTable, new int[] { 0, 2, 3 });
            return;
        }
        /// <summary>
        /// 查询客户
        /// </summary>
        public void QueryClient()
        {
            DataTable dt = new ExtractInventoryTool_ClientBLL().QueryClient();
            QueryClientCallbackDel del = QueryClientCallback;
            dataGridView1.BeginInvoke(del, dt);
            return;
        }
        #endregion

        #region 查询物料
        /// <summary>
        /// 查询客户回调委托
        /// </summary>
        /// <param name="dt">结果dt</param>
        delegate void QueryMaterialCallbackDel(DataTable dt);
        /// <summary>
        /// 查询客户回调事件
        /// </summary>
        /// <param name="dt"></param>
        public void QueryMaterialCallback(DataTable dt)
        {
            _materialTable.Clear();//清除所有数据，行不保留
            foreach (DataRow row in dt.Rows)
            {
                _materialTable.Rows.Add(
                    row["Oid"],
                    row["Name"],
                    row["Code"],
                    row["Supplier"],
                    row["SupplierCode"],
                    row["Client"]
                    );
            }
            BindGrid(dataGridView2, _materialTable, new int[] { 0, 5 });
            return;
        }
        /// <summary>
        /// 查询当前客户的物料
        /// </summary>
        public void QueryMaterialByClientID(string clientID)
        {
            DataTable dt = new ExtractInventoryTool_MaterialBLL().QueryMaterialByClientID(clientID);
            QueryMaterialCallbackDel del = QueryMaterialCallback;
            dataGridView2.BeginInvoke(del, dt);
            return;
        }
        #endregion

        #region 删除物料
        delegate void DeleteRecordsCallbackDel(string errorMessage);
        public void DeleteRecordsCallback(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            DataGridViewSelectedRowCollection clientRowCollection = dataGridView1.SelectedRows;
            QueryMaterialByClientID(clientRowCollection[0].Cells[0].Value.ToString());
        }
        public void DeleteMaterialRecords(string tableName, List<string> ids)
        {
            string errorMessage = string.Empty;
            int result = new ExtractInventoryTool_MaterialBLL().DeleteRecords(tableName, ids, out errorMessage);
            DeleteRecordsCallbackDel del = DeleteRecordsCallback;
            dataGridView1.BeginInvoke(del, errorMessage);
        }

        #endregion

        delegate void SaveMaterialExcelCallbackDel(bool isSucceed, string errorMessage);
        public void SaveMaterialExcelCallback(bool isSucceed, string errorMessage)
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
                Task.Run(() => SaveMaterialExcel(_excelData, true));
                return;
            }
            _excelData = null;//初始化导入数据
            MessageBox.Show("导入成功！", "Success");
            DataGridViewSelectedRowCollection clientRowCollection = dataGridView1.SelectedRows;
            QueryMaterialByClientID(clientRowCollection[0].Cells[0].Value.ToString());
        }
        #region 导入物料备案
        delegate void ImportMaterialExcelCallBackDel(List<ExtractInventoryTool_Material> excelData, string errorMessage);
        public void ImportMaterialExcelCallBack(List<ExtractInventoryTool_Material> excelData, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                LoadingHelper.CloseForm();
                MessageBox.Show(errorMessage, "Error");
                return;
            }
            Task.Run(() => SaveMaterialExcel(_excelData, false));
        }
        public void ImportMaterialExcel(string fileName, int clientOid , string clientUniqueCode)
        {
            _excelData = null;
            string errorMessage = string.Empty;
            _excelData = new NPOIHelper().ExcelToMaterialList(string.Empty, false, fileName, clientOid, clientUniqueCode,out errorMessage);
            ImportMaterialExcelCallBackDel del = ImportMaterialExcelCallBack;
            this.BeginInvoke(del, _excelData, errorMessage);
        }

        public void SaveMaterialExcel(List<ExtractInventoryTool_Material> excelData, bool isCover)
        {
            string errorMessage = string.Empty;
            bool importResult = false;
            importResult = new ExtractInventoryTool_MaterialBLL().ImportMaterial(_excelData, isCover, out errorMessage);
            SaveMaterialExcelCallbackDel del = SaveMaterialExcelCallback;
            this.BeginInvoke(del, importResult, errorMessage);
        }
        #endregion

    }
}
