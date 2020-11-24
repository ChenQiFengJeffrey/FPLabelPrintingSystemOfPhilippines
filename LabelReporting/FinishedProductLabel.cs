using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FPLabelData;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace LabelReporting
{
    public partial class FinishedProductLabel : DevExpress.XtraReports.UI.XtraReport
    {
        private FinishedProductLabelDTO _dto = null;
        private List<KeyValuePair<string, string>> _snList = null;
        public FinishedProductLabel()
        {
            InitializeComponent();
        }
        public FinishedProductLabel(FinishedProductLabelDTO dto, List<KeyValuePair<string, string>> snList)
        {
            InitializeComponent();
            _dto = dto;
            _snList = snList;
        }

        public void FinishedProductLabel_Load1()
        {
            List<SN> list = new List<SN>();
            list.Add(new SN() { Name = "1" });
            list.Add(new SN() { Name = "2" });
            Detail.DataBindings.Add("Text", list, "Name");
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        //    public void FinishedProductLabel_Load()
        //    {
        //        xrLabel12.Text = _dto.ID;
        //        #region Package
        //        Package a = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("A"));
        //    xrCheckBox1.Checked = a == null ? false : a.IsChecked;
        //        Package b = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("B"));
        //    xrCheckBox2.Checked = b == null ? false : b.IsChecked;
        //        Package c = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("C"));
        //    xrCheckBox3.Checked = c == null ? false : c.IsChecked;
        //        Package d = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("D"));
        //    xrCheckBox4.Checked = d == null ? false : d.IsChecked;
        //        Package e = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("E"));
        //    xrCheckBox5.Checked = e == null ? false : e.IsChecked;
        //        Package f = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("F"));
        //    xrCheckBox6.Checked = f == null ? false : f.IsChecked;
        //        Package g = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("G"));
        //    xrCheckBox7.Checked = g == null ? false : g.IsChecked;
        //        Package h = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("H"));
        //    xrCheckBox8.Checked = h == null ? false : h.IsChecked;
        //        Package home = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("HOME"));
        //    xrCheckBox9.Checked = home == null ? false : home.IsChecked;
        //        Package sme = _dto.PackageList.FirstOrDefault(p => p.Name.Trim().ToUpper().Equals("SME"));
        //    xrCheckBox10.Checked = sme == null ? false : sme.IsChecked;
        //        #endregion
        //        #region SN
        //        xrLabel7.Text = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("ONU")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("ONU")).Value : "";
        //        xrLabel8.Text = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("VDSL")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("VDSL")).Value : "";
        //        xrLabel9.Text = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("TELSET")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("TELSET")).Value : "";
        //        xrLabel10.Text = _snList.Exists(k => k.Key.Trim().ToUpper().Equals("BIZBOX")) ? _snList.FirstOrDefault(k => k.Key.Trim().ToUpper().Equals("BIZBOX")).Value : "";
        //        #endregion
        //        #region Good
        //        foreach (var item in _dto.goodList)
        //        {
        //            XRTableRow lastRow = xrTable4.Rows.LastRow;
        //    XRTableRow newRow = xrTable4.InsertRowBelow(lastRow);
        //    newRow.BackColor = Color.Transparent;
        //            newRow.Cells[0].Text = item.MaterialNum;
        //            newRow.Cells[1].Text = "PC";
        //            newRow.Cells[2].Text = item.QTY.ToString();
        //        }
        //#endregion
        //#region Barcode
        //StringBuilder barcodeStrbd = new StringBuilder();
        //barcodeStrbd.Append("(")
        //            .Append("").Append("|")//库位
        //            .Append("").Append("|")//ASN
        //            .Append(_dto.FinishedProductNum).Append("|")//物料号
        //            .Append("").Append("|")//???
        //            .Append("1").Append("|")//数量
        //            .Append("").Append("|")//批次
        //            .Append("烽火").Append("|")//供应商名称
        //            .Append("FH").Append("|")//供应商编码
        //            .Append("").Append("|")//箱数
        //            .Append("").Append("|")//总箱数
        //            .Append(_dto.ID).Append("|")//唯一码
        //            .Append(xrLabel7.Text).Append("|")//sunumber|ONU
        //            .Append(xrLabel8.Text).Append("|")//productionlot|VDSL
        //            .Append(xrLabel9.Text).Append("|")//rawmateriallot|TELSET
        //            .Append(xrLabel10.Text)             //heatnumber|BIZBOX
        //            .Append(")");
        //xrBarCode1.Text = barcodeStrbd.ToString();
        //        #endregion
        //    }

        private void FinishedProductLabel_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FinishedProductLabel_Load1();
        }
    }
}
