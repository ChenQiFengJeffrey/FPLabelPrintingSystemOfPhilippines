using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelPrintingSystemOfPhilippines.ColumnNameConfig
{
    public class GoodSet
    {
        public int Oid { get; set; }
        public int FinishedProductNum { get; set; }//成品料号
        //public int FinishedProductVariety { get; set; }//成品种类
        public string MaterialNum { get; set; }//原材料号
        public int QTY { get; set; }//单套用量
        public static List<GoodSet> DataTableToList(DataTable dt)
        {
            List<GoodSet> list = new List<GoodSet>();
            if (dt.Rows.Count == 0)
                return list;
            foreach (DataRow row in dt.Rows)
            {
                GoodSet set = new GoodSet();
                set.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                set.FinishedProductNum = row["FinishedProductNum"] == DBNull.Value ? 0 : Convert.ToInt32(row["FinishedProductNum"]);
                set.MaterialNum = row["MaterialNum"] == DBNull.Value ? "" : (string)row["MaterialNum"];
                set.QTY = row["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(row["QTY"]);
                list.Add(set);
            }
            return list;
        }
    }
}
