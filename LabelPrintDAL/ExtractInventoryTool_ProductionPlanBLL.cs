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
    public class ExtractInventoryTool_ProductionPlanBLL : ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 查询生产计划
        /// </summary>
        public DataTable QueryProductionPlan()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select pp.*,date(pp.ProductionDate) as ppProductionDate,cl.UniqueCode as ClientCode from ProductionPlan pp inner join Client cl on pp.Client=cl.Oid order by pp.UpdateTime desc");
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
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
        public DataTable QueryProductionPlan(string limit, string offset, string whereStr, out int totalCount)
        {
            lock (lockObj)
            {
                try
                {
                    totalCount = 0;
                    string countStr = " count(1) as TotalCount ";//用来计算总记录数
                    string selectStr = " pp.*,date(pp.ProductionDate) as ppProductionDate,cl.UniqueCode as ClientCode ";
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from ProductionPlan pp inner join Client cl on pp.Client=cl.Oid ")
                        .Append(" where 1=1 ")
                        .Append(whereStr)
                        .Append(" order by pp.UpdateTime desc");
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
        /// 删除计划
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
                    StringBuilder deleteStrbd = new StringBuilder();
                    deleteStrbd.Append("delete from ")
                        .Append(tableName)
                        .Append(" where Oid in ( ");
                    foreach (string id in ids)
                    {
                        deleteStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    deleteStrbd.Remove(deleteStrbd.Length - 1, 1);
                    deleteStrbd.Append(" ) ");
                    return new SQLiteHelper().ExecuteNonQuery(deleteStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void InsertOrUpdateClient(ExtractInventoryTool_Client client)
        {
            lock (lockObj)
            {
                try
                {
                    if (client == null)
                    {
                        MessageBox.Show("客户信息为空", "Error");
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,client.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String,client.Name),
                    SQLiteHelper.MakeSQLiteParameter("@Code", DbType.String,client.UniqueCode),
                    SQLiteHelper.MakeSQLiteParameter("@Remark", DbType.String,client.Remark),
                    SQLiteHelper.MakeSQLiteParameter("@RegexRule", DbType.String,client.RegexRule)
                    };
                    paramList.Add(parameter);
                    if (client.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into Client (Name,Code,Remark,RegexRule) ")
                            .Append(@"values ( ")
                            .Append(@"@Name,@Code,@Remark,@RegexRule ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update Client set Name=@Name,Code=@Code,Remark=@Remark,RegexRule=@RegexRule ")
                            .Append(@" WHERE Oid=@Oid");
                    }
                    new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                    return;
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
        public void ClearTable(string tableName,out string errorMessage)
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

        /// <summary>
        /// 导入生产计划
        /// </summary>
        /// <param name="productionPlanList"></param>
        /// <returns></returns>
        public bool ImportProductionPlan(List<ExtractInventoryTool_ProductionPlan> productionPlanList, bool isCover, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (productionPlanList == null || productionPlanList.Count == 0)
                    {
                        errorMessage = "生产计划为空";
                        return false;
                    }
                    //先查询出所有的生产计划唯一码
                    DataTable allMaterial = GetAllUniqueCode("ProductionPlan");
                    List<ExtractInventoryTool_ProductionPlan> allProductionPlanList = new List<ExtractInventoryTool_ProductionPlan>();
                    foreach (DataRow row in allMaterial.Rows)
                    {
                        allProductionPlanList.Add(new ExtractInventoryTool_ProductionPlan()
                        {
                            Oid = Int32.Parse(row["Oid"].ToString()),
                            UniqueCode = row["UniqueCode"].ToString()
                        });
                    }
                    StringBuilder insertStrbd = new StringBuilder();
                    StringBuilder updateStrbd = new StringBuilder();
                    List<SQLiteParameter[]> insertParamList = new List<SQLiteParameter[]>();
                    List<SQLiteParameter[]> updateParamList = new List<SQLiteParameter[]>();
                    foreach (var productionPlan in productionPlanList)
                    {
                        ExtractInventoryTool_ProductionPlan exitsPP = allProductionPlanList.FirstOrDefault(m => m.UniqueCode.Trim().Equals(productionPlan.UniqueCode.Trim()));
                        if (exitsPP != null && exitsPP.Oid != 0)//导入数据如果表里有就更新，没有就新建
                        {
                            if (!isCover)
                            {
                                errorMessage = "存在重复数据，是否覆盖？";
                                return true;
                            }
                            productionPlan.Oid = exitsPP.Oid;
                        }
                        SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,productionPlan.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@VehicleModelCode", DbType.String,productionPlan.VehicleModelCode),
                            SQLiteHelper.MakeSQLiteParameter("@ProductionDate", DbType.Date,productionPlan.ProductionDate),
                            SQLiteHelper.MakeSQLiteParameter("@UnitNum", DbType.Int32,productionPlan.UnitNum),
                            SQLiteHelper.MakeSQLiteParameter("@UpdateTime", DbType.DateTime,productionPlan.UpdateTime),
                            SQLiteHelper.MakeSQLiteParameter("@Client", DbType.Int32,productionPlan.Client),
                            SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,productionPlan.UniqueCode)
                        };
                        if (exitsPP != null && exitsPP.Oid != 0)
                        {
                            updateParamList.Add(parameter);
                        }
                        else
                        {
                            insertParamList.Add(parameter);
                        }
                    }
                    //添加新数据
                    insertStrbd.Append(@"Insert into ProductionPlan (VehicleModelCode,ProductionDate,UnitNum,UpdateTime,Client,UniqueCode) ")
                        .Append(@"values ( ")
                        .Append(@"@VehicleModelCode,@ProductionDate,@UnitNum,@UpdateTime,@Client,@UniqueCode ")
                        .Append(@")");
                    new SQLiteHelper().ExecuteNonQueryBatch(insertStrbd.ToString(), insertParamList);
                    updateStrbd.Append(@"Update ProductionPlan set VehicleModelCode=@VehicleModelCode,ProductionDate=@ProductionDate,UnitNum=@UnitNum,UpdateTime=@UpdateTime,Client=@Client,UniqueCode=@UniqueCode ")
                                        .Append(@" WHERE Oid=@Oid");
                    new SQLiteHelper().ExecuteNonQueryBatch(updateStrbd.ToString(), updateParamList);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
