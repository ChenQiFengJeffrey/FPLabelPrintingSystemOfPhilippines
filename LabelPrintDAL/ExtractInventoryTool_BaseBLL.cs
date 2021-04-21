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
    public class ExtractInventoryTool_BaseBLL
    {
        protected static object lockObj = new object();
        /// <summary>
        /// 创建排序子语句
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public virtual string MakeUpOrderStr(List<KeyValuePair<string, string>> orderList)
        {
            StringBuilder resulet = new StringBuilder();
            if (orderList == null || orderList.Count <= 0)
                return resulet.ToString();
            return resulet.ToString();
        }

        /// <summary>
        /// 创建分页子语句
        /// </summary>
        /// <param name="limit">页面个数</param>
        /// <param name="offset">起始页</param>
        /// <returns></returns>
        public virtual string MakeUpPageStr(string limit, string offset)
        {
            if (string.IsNullOrEmpty(limit) || string.IsNullOrEmpty(offset))
                return string.Empty;
            StringBuilder resulet = new StringBuilder();
            resulet.Append(" limit ").Append(limit).Append(" offset ").Append(offset);
            return resulet.ToString();
        }

        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <param name="countSelectStr"></param>
        /// <returns></returns>
        public virtual int TotalCount(string countSelectStr)
        {
            int count = 0;
            var countdt = new SQLiteHelper().ExecuteScalar(countSelectStr);
            count = Int32.TryParse(countdt.ToString(), out count) ? count : 0;
            return count;
        }

        /// <summary>
        /// 获取表的Oid和UniqueCode
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetAllUniqueCode(string tableName, string columnName = "")
        {
            lock (lockObj)
            {
                try
                {
                    if (string.IsNullOrEmpty(columnName))
                        columnName = "UniqueCode";
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select Oid,")
                                    .Append(columnName)
                                    .Append(" from ")
                                    .Append(tableName);
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 唯一码是否存在
        /// </summary>
        /// <param name="uniqueCode">唯一码</param>
        /// <param name="tableName"表名</param>
        /// <returns></returns>
        public bool IsExistsUniqueCode(string uniqueCode, string tableName, string oid="")
        {
            lock (lockObj)
            {
                try
                {
                    int count = 0;
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select count(1) from ")
                                    .Append(tableName)
                                    .Append(" where UniqueCode='")
                                    .Append(uniqueCode)
                                    .Append("'");
                    if (!string.IsNullOrEmpty(oid))
                    {
                        queryStrbd.Append(" and Oid!=").Append(oid);
                    }
                    var countobj = new SQLiteHelper().ExecuteScalar(queryStrbd.ToString());
                    count = Int32.TryParse(countobj.ToString(), out count) ? count : 0;
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 清空表
        /// </summary>
        /// <param name="tableName"></param>
        public void ClearTable(string tableName, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    StringBuilder deleteStrbd = new StringBuilder();
                    deleteStrbd.Append("delete from ")
                        .Append(tableName)
                        .Append(" ; ")
                        .Append("update sqlite_sequence set seq = 0 where name = '")
                        .Append(tableName)
                        .Append("';");
                    new SQLiteHelper().ExecuteNonQuery(deleteStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
