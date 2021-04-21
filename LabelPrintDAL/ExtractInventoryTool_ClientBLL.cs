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
    public class ExtractInventoryTool_ClientBLL : ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 分页查询客户
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        public DataTable QueryClient(string limit, string offset, out int totalCount)
        {
            lock (lockObj)
            {
                try
                {
                    totalCount = 0;
                    string countStr = " count(1) as TotalCount ";//用来计算总记录数
                    string selectStr = " * ";//查询语句需要查询的字段
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from Client order by Oid desc ");
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
        /// 查询客户
        /// </summary>
        public DataTable QueryClient()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from Client order by Oid desc ");
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int DeleteRecords(string tableName, List<string> ids,out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    //判断是否有该客户的物料，如果有，则不能删除
                    StringBuilder hasMaterialStrbd = new StringBuilder();
                    StringBuilder idStrbd = new StringBuilder();
                    foreach (string id in ids)
                    {
                        idStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    idStrbd.Remove(idStrbd.Length - 1, 1);
                    hasMaterialStrbd.Append("select count(*) from Material where Client in ( ")
                                                .Append(idStrbd).Append(" ) ");
                    var hasMaterial= new SQLiteHelper().ExecuteScalar(hasMaterialStrbd.ToString());
                    if (Convert.ToInt32(hasMaterial) > 0)
                    {
                        errorMessage = "存在与待删除客户相关联的物料，不能删除";
                        return 0;
                    }
                    StringBuilder deleteStrbd = new StringBuilder();
                    deleteStrbd.Append("delete from ")
                        .Append(tableName)
                        .Append(" where Oid in ( ");
                    deleteStrbd.Append(idStrbd);
                    deleteStrbd.Append(" ) ");
                    return new SQLiteHelper().ExecuteNonQuery(deleteStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int InsertOrUpdateClient(ExtractInventoryTool_Client client,out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (client == null)
                    {
                        errorMessage = "客户信息为空";
                        return 0;
                    }
                    string oid = client.Oid == 0 ? string.Empty : client.Oid.ToString();
                    if (IsExistsUniqueCode(client.UniqueCode, "Client", oid))
                    {
                        errorMessage = "该客户已存在，无法添加相同客户";
                        return 0;
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,client.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String,client.Name),
                    SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,client.UniqueCode),
                    SQLiteHelper.MakeSQLiteParameter("@Remark", DbType.String,client.Remark),
                    SQLiteHelper.MakeSQLiteParameter("@RegexRule", DbType.String,client.RegexRule)
                    };
                    paramList.Add(parameter);
                    if (client.Oid == 0)
                    {
                       
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into Client (Name,UniqueCode,Remark,RegexRule) ")
                            .Append(@"values ( ")
                            .Append(@"@Name,@UniqueCode,@Remark,@RegexRule ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update Client set Name=@Name,UniqueCode=@UniqueCode,Remark=@Remark,RegexRule=@RegexRule ")
                            .Append(@" WHERE Oid=@Oid");
                    }
                    new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    return 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
