using DevExpress.XtraReports.UI;
using FPLabelData;
using LabelPrintDAL;
using LabelReporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FPLabelPrintingWcfService;
using FPLabelPrintingClient.LabelPrintingServiceReference;
using System.Configuration;
using System.Text.RegularExpressions;

namespace FPLabelPrintingClient
{
    public partial class LabelPrintingForm : Form
    {
        private PrintSet _currentPrintSet = null;
        private RoSet _currentRoSet = null;
        private List<KeyValuePair<string, string>> _snList = new List<KeyValuePair<string, string>>();
        private List<GoodSet> _currentGoodSetList = null;
        private string _workStation = ConfigurationManager.AppSettings["WorkStation"].Trim();
        private string _supplierName = ConfigurationManager.AppSettings["SupplierName"].Trim();
        private string _supplierCode = ConfigurationManager.AppSettings["SupplierCode"].Trim();
        private string _idStr = string.Empty;
        public LabelPrintingForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string text = FinishedProductNum.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                Speecher("Finished Product Number is null");
                return;
            }
            GetPrintSetAndGoodSet(text);
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            LabelPrintClear();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void LabelPrintClear()
        {
            _currentPrintSet = null;
            _currentRoSet = null;
            _currentGoodSetList = null;
            _snList.Clear();
            _idStr = "";
            button4.BackColor = Color.Red;
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
            labelID.Clear();
            FinishedProductNum.Focus();
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
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
            if (_currentRoSet == null)
            {
                Speecher("Ro# Config is null");
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

        private void LabelPrintingForm_Load(object sender, EventArgs e)
        {
            LabelPrintClear();
        }

        private void FinishedProductNum_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            //1.获取成品料号
            TextBox tb = sender as TextBox;
            string text = tb.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                Speecher("Finished Product Number is Null");
                return;
            }
            GetPrintSetAndGoodSet(text);
        }


        public void GetPrintSetAndGoodSet(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            #region 这里做流程控制，在同一个线程中操作
            try
            {
                using (PrintSetServiceClient client = new PrintSetServiceClient())
                {
                    //2、获取打印配置
                    DataTable psdt = client.GetPrintSetByFPNum(text);

                    if (psdt == null || psdt.Rows.Count == 0)
                    {
                        Speecher("Print Config is null");
                        return;
                    }
                    //3、获取成品配置
                    DataTable gsdt = client.GetGoodSetByFPNum(text);
                    if (gsdt == null || gsdt.Rows.Count == 0)
                    {
                        Speecher("Goods Config is null");
                        return;
                    }
                    //4、获取Ro配置
                    DataTable rsdt = client.GetRoSetByFPNum(text);
                    if (rsdt == null || rsdt.Rows.Count == 0)
                    {
                        Speecher("Ro# Config is null");
                        return;
                    }
                    button4.BackColor = Color.Green;
                    List<PrintSet> printsetList = PrintSet.DataTableToList(psdt);
                    List<RoSet> rosetList = RoSet.DataTableToList(rsdt);
                    List<GoodSet> goodsetList = GoodSet.DataTableToList(gsdt);
                    _currentPrintSet = null;
                    _currentPrintSet = printsetList.OrderByDescending(p=>p.CreateTime).First();
                    _currentRoSet = null;
                    _currentRoSet = rosetList.OrderByDescending(r=>r.Oid).First();
                    _currentGoodSetList = null;
                    _currentGoodSetList = goodsetList;
                    GotoNextTextBox(FinishedProductNum);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetPrintSetAndGoodSet", ex);
            }
            #endregion
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
                    if (_snList.Exists(k => k.Key.Trim().ToUpper().Equals(box.Name.Trim().ToUpper())))
                    {
                        KeyValuePair<string, string> sn = _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals(box.Name.Trim().ToUpper()));
                        _snList.Remove(sn);
                    }
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
            labelID.Focus();
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
            result.VVDSL = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("VVDSL")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("VVDSL")).Value : "";
            result.TELSET = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("TELSET")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("TELSET")).Value : "";
            result.BIZBOX = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("BIZBOX")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("BIZBOX")).Value : "";
            #endregion
            #region Good
            result.GoodList = _currentGoodSetList;
            #endregion
            #region Ro
            result.RoNumber = _currentRoSet.RoNumber;
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
                        .Append(result.VVDSL).Append("|")//productionlot|VVDSL
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
            //LabelPrintClear();
            FinishedProductNum.ReadOnly = true;
            ONU.ReadOnly = true;
            VVDSL.ReadOnly = true;
            TELSET.ReadOnly = true;
            BIZBOX.ReadOnly = true;
            return;
        }

        public void InsertLabelRecord(List<FinishedProductLabelDTO> dtoList)
        {
            try
            {
                using (PrintSetServiceClient client = new PrintSetServiceClient())
                {
                    FinishedProductLabelDTO[] dtoAry = dtoList.ToArray();
                    client.InsertLabelRecord(dtoAry);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetPrintSetAndGoodSet", ex);
            }
        }
        public void Speecher(string text)
        {
            //using (SpeechSynthesizer speech = new SpeechSynthesizer())
            //{
            //    speech.Rate = 0;  //语速
            //    speech.Volume = 100;  //音量
            //    speech.Speak(text);
            //}
            MessageBox.Show(text, "Warning");
        }

        private void ONU_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the ONU is empty!");
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
                Speecher("The text of the VVDSL is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("VVDSL", box.Text));
            GotoNextTextBox(VVDSL);
        }

        private void TELSET_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the TELSET is empty!");
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
                Speecher("The text of the BIZBOX is empty!");
                return;
            }
            _snList.Add(new KeyValuePair<string, string>("BIZBOX", box.Text));
            GotoNextTextBox(BIZBOX);
        }

        private void labelID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            TextBox box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                Speecher("The text of the LabelID is empty!");
                return;
            }
            //加入标签打印的最终确认动作
            if (string.IsNullOrWhiteSpace(_idStr))
                return;
            bool isCorrect = box.Text.Contains(_idStr);
            if (isCorrect)
            {
                LabelPrintClear();
            }
            else
            {
                labelID.Clear();
                StringBuilder warnStr = new StringBuilder();
                warnStr.Append("The current label ID is ")
                    .Append(_idStr)
                    .Append(" , please reconfirm!");
                Speecher(warnStr.ToString());
            }
            return;
        }
    }
}
