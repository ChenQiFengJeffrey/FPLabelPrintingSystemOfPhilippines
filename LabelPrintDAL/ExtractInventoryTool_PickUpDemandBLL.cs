using FPLabelData.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrintDAL
{
    public class ExtractInventoryTool_PickUpDemandBLL : ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 分页带条件查询生产计划
        /// </summary>
        public DataTable QueryPickUpDemand(string limit, string offset, string whereStr, out int totalCount)
        {
            lock (lockObj)
            {
                try
                {
                    totalCount = 0;
                    string countStr = " count(1) as TotalCount ";//用来计算总记录数
                    string selectStr = " *,datetime('now') as QueryTime ";
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from PickUpDemandDetails ")
                        .Append(" where 1=1 ")
                        .Append(whereStr);
                        //.Append(" order by pp.UpdateTime desc");
                    //先查询总记录数
                    totalCount = TotalCount(string.Format(queryStrbd.ToString(), countStr));
                    //再查询记录
                    queryStrbd.Append(MakeUpPageStr(limit, offset));
                    return new SQLiteHelper().ExecuteQuery(string.Format(queryStrbd.ToString(), selectStr));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 分页带条件查询生产计划
        /// </summary>
        public DataTable QueryPickUpDemand(string whereStr)
        {
            lock (lockObj)
            {
                try
                {
                    string selectStr = " *,datetime('now') as QueryTime ";
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from PickUpDemandDetails ")
                        .Append(" where 1=1 ")
                        .Append(whereStr);
                    //.Append(" order by pp.UpdateTime desc");
                    return new SQLiteHelper().ExecuteQuery(string.Format(queryStrbd.ToString(), selectStr));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
