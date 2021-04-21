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
    public partial class Form_BOMEditor : Form
    {
        #region 私有属性
        private ExtractInventoryTool_BOM _bom = null;
        private List<ExtractInventoryTool_MaterialExtension> _materialList = null;
        #endregion
        public Form_BOMEditor(ExtractInventoryTool_BOM bom)
        {
            InitializeComponent();
            _bom = bom;
        }

        /// <summary>
        /// 物料下拉框选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ExtractInventoryTool_MaterialExtension selectedPrint = (ExtractInventoryTool_MaterialExtension)comboBox.SelectedItem;
            if (selectedPrint == null)
            {
                textBox3.Text = textBox4.Text = textBox5.Text = textBox6.Text = textBox7.Text = string.Empty;
                return;
            }
            #region 显示物料相关信息
            textBox5.Text = selectedPrint.Name;
            textBox3.Text = selectedPrint.SupplierCode;
            textBox4.Text = selectedPrint.Supplier;
            textBox7.Text = selectedPrint.ClientCode;
            textBox6.Text = selectedPrint.ClientName; 
            #endregion
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
                MessageBox.Show("车辆代码不能为空", "Warning");
                return;
            }
            if (numericUpDown1.Value<=0)
            {
                MessageBox.Show("单位用量必须大于1", "Warning");
                return;
            }
            #endregion

            #region 装载Ro
            ExtractInventoryTool_BOM bom = new ExtractInventoryTool_BOM();
            int oid = 0;
            bom.Oid = int.TryParse(textBox1.Text.Trim(), out oid) ? oid : 0;
            bom.Material = ((ExtractInventoryTool_MaterialExtension)comboBox1.SelectedItem).Oid;
            bom.VehicleModelCode = textBox2.Text.Trim();
            bom.UnitUsage = Convert.ToInt32(numericUpDown1.Value);
            bom.UpdateTime = DateTime.Now;
            bom.UniqueCode = _materialList.First(m => m.Oid == bom.Material).UniqueCode + "v" + bom.VehicleModelCode;
            #endregion
            Task.Run(() => InsertOrUpdateMaterial(bom));
            return;
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

        private void Form_BOMEditor_Load(object sender, EventArgs e)
        {
            #region 显示待更新的记录
            if (_bom != null && _bom.Oid != 0)
            {
                textBox1.Text = _bom.Oid.ToString();
                textBox2.Text = _bom.VehicleModelCode;
                numericUpDown1.Value = _bom.UnitUsage;
            }
            #endregion
            #region 加载物料配置
            Task.Run(() => QueryMaterial());
            #endregion
            return;
        }

        private void Form_BOMEditor_Activated(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        #region 加载物料配置
        delegate void QueryMaterialCallbackDel(List<ExtractInventoryTool_MaterialExtension> materialList);
        private void QueryMaterial()
        {
            try
            {
                DataTable dt = new ExtractInventoryTool_MaterialBLL().QueryMaterialExtension();
                _materialList = new List<ExtractInventoryTool_MaterialExtension>();
                foreach (DataRow row in dt.Rows)
                {
                    ExtractInventoryTool_MaterialExtension material = new ExtractInventoryTool_MaterialExtension();
                    material.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                    material.Name = row["Name"] == DBNull.Value ? "" : (string)row["Name"];
                    material.Code = row["Code"] == DBNull.Value ? "" : (string)row["Code"];
                    material.Supplier = row["Supplier"] == DBNull.Value ? "" : (string)row["Supplier"];
                    material.SupplierCode = row["SupplierCode"] == DBNull.Value ? "" : (string)row["SupplierCode"];
                    material.Client = row["Client"] == DBNull.Value ? 0 : Convert.ToInt32(row["Client"]);
                    material.ClientName = row["ClientName"] == DBNull.Value ? "" : (string)row["ClientName"];
                    material.ClientCode = row["ClientCode"] == DBNull.Value ? "" : (string)row["ClientCode"];
                    material.UniqueCode = row["UniqueCode"] == DBNull.Value ? "" : (string)row["UniqueCode"];
                    _materialList.Add(material);
                }
                QueryMaterialCallbackDel del = QueryMaterialCallback;
                comboBox1.BeginInvoke(del, _materialList);
                return;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("QueryClient", ex);
            }

        }
        private void QueryMaterialCallback(List<ExtractInventoryTool_MaterialExtension> materialList)
        {
            comboBox1.DataSource = materialList;
            comboBox1.DisplayMember = "NameCode";
            comboBox1.ValueMember = "Oid";
            comboBox1.SelectedItem = null;
            if (_bom != null)
            {
                ExtractInventoryTool_MaterialExtension selectedPrint = materialList.FirstOrDefault(p => p.Oid == _bom.Material);
                if (selectedPrint != null)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(selectedPrint);
                }
            }
            return;
        }

        #endregion

        #region 插入和更新BOM
        delegate void InsertOrUpdateBOMCallBackDel(string errorMessage);
        /// <summary>
        /// 添加/更新物料配置
        /// </summary>
        /// <param name="bom"></param>
        private void InsertOrUpdateMaterial(ExtractInventoryTool_BOM bom)
        {
            try
            {
                string errorMessage = string.Empty;
                new ExtractInventoryTool_BOMBLL().InsertOrUpdateBOM(bom, out errorMessage);
                InsertOrUpdateBOMCallBackDel del = InsertOrUpdateBOMCallBack;
                this.BeginInvoke(del, errorMessage);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void InsertOrUpdateBOMCallBack(string errorMessage)
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
        #endregion
    }
}
