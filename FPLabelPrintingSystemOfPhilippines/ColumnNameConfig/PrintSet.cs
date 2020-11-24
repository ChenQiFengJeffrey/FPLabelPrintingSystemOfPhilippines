using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelPrintingSystemOfPhilippines.ColumnNameConfig
{
    public class PrintSet
    {
        public int Oid { get; set; }
        public string FinishedProductNum { get; set; }
        public string SN { get; set; }
        public string Package { get; set; }
        public DateTime CreateTime { get; set; }
        public List<SN> SNList
        {
            get
            {
                return JsonConvert.DeserializeObject<List<SN>>(SN).Where(s => s.IsUsed == true).OrderBy(s => s.Order).ToList();
            }
        }
        public List<Package> PackageList
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Package>>(Package).OrderBy(s => s.Order).ToList();
            }
        }
        public static List<PrintSet> DataTableToList(DataTable dt)
        {
            List<PrintSet> list = new List<PrintSet>();
            if (dt.Rows.Count == 0)
                return list;
            foreach (DataRow row in dt.Rows)
            {
                PrintSet set = new PrintSet();
                set.Oid = row["Oid"] == DBNull.Value ? 0 : Convert.ToInt32(row["Oid"]);
                set.FinishedProductNum = row["FinishedProductNum"] == DBNull.Value ? "" : (string)row["FinishedProductNum"];
                set.SN = row["SN"] == DBNull.Value ? "" : (string)row["SN"];
                set.Package = row["Package"] == DBNull.Value ? "" : (string)row["Package"];
                list.Add(set);
            }
            return list;
        }


    }
}
