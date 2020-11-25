using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelData
{
    public class RoSet
    {
        public int Oid { get; set; }
        public int FinishedProductNum { get; set; }//成品料号
        public string RoNumber { get; set; }//RO号(订单追溯码)

        public static List<RoSet> DataTableToList(DataTable dt)
        {
            List<RoSet> list = new List<RoSet>();
            if (dt.Rows.Count == 0)
                return list;
            foreach (DataRow row in dt.Rows)
            {
                RoSet set = new RoSet();
                set.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                set.FinishedProductNum = row["FinishedProductNum"] == DBNull.Value ? 0 : Convert.ToInt32(row["FinishedProductNum"]);
                set.RoNumber = row["RoNumber"] == DBNull.Value ? "" : (string)row["RoNumber"];
                list.Add(set);
            }
            return list;
        }
    }
}
