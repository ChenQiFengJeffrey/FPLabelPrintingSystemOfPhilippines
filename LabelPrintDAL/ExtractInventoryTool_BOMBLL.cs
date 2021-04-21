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
    public class ExtractInventoryTool_BOMBLL: ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 查询BOM
        /// </summary>
        public DataTable QueryBOM()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select bo.Oid, bo.VehicleModelCode, mt.Oid as Material, mt.Name as MaterialName, mt.Code as MaterialCode, mt.Supplier as SupplierName, mt.SupplierCode as SupplierCode, cl.Name as ClientName, cl.Code as ClientCode, bo.UnitUsage, bo.UpdateTime from BOM bo inner join Material mt on bo.Material=mt.Oid inner join Client cl on mt.Client=cl.Oid order by bo.Oid desc");
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 分页查询BOM
        /// </summary>
        public DataTable QueryBOM(string limit, string offset, string whereStr, out int totalCount)
        {
            lock (lockObj)
            {
                try
                {
                    totalCount = 0;
                    string countStr = " count(1) as TotalCount ";//用来计算总记录数
                    string selectStr = " bo.Oid, bo.VehicleModelCode, mt.Oid as Material, mt.Name as MaterialName, mt.Code as MaterialCode, mt.Supplier as SupplierName, mt.SupplierCode as SupplierCode, cl.Name as ClientName, cl.UniqueCode as ClientCode, bo.UnitUsage, bo.UpdateTime ";//查询语句需要查询的字段
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from BOM bo inner join Material mt on bo.Material=mt.Oid inner join Client cl on mt.Client=cl.Oid ")
                                        .Append(" where 1=1 ")
                                        .Append(whereStr)
                                        .Append(" order by bo.Oid desc");
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
                    //判断是否有该BOM的生产计划，如果有，则不能删除
                    StringBuilder hasPlanStrbd = new StringBuilder();
                    StringBuilder idStrbd = new StringBuilder();
                    foreach (string id in ids)
                    {
                        idStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    idStrbd.Remove(idStrbd.Length - 1, 1);
                    hasPlanStrbd.Append("select count(*) from ProductionPlan where exists (select VehicleModelCode from BOM where Oid in ( ")
                                                .Append(idStrbd).Append(" )) ");
                    var hasPlan = new SQLiteHelper().ExecuteScalar(hasPlanStrbd.ToString());
                    if (Convert.ToInt32(hasPlan) > 0)
                    {
                        errorMessage = "存在与待删除BOM相关联的生产计划，不能删除";
                        return 0;
                    }
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

        public void InsertOrUpdateBOM(ExtractInventoryTool_BOM bom,out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (bom == null)
                    {
                        errorMessage = "BOM信息为空";
                        return;
                    }
                    string oid = bom.Oid == 0 ? string.Empty : bom.Oid.ToString();
                    if (IsExistsUniqueCode(bom.UniqueCode, "BOM", oid))
                    {
                        errorMessage = "该BOM已存在，无法添加相同BOM";
                        return;
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,bom.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@VehicleModelCode", DbType.String,bom.VehicleModelCode),
                    SQLiteHelper.MakeSQLiteParameter("@Material", DbType.Int32,bom.Material),
                    SQLiteHelper.MakeSQLiteParameter("@UnitUsage", DbType.Int32,bom.UnitUsage),
                    SQLiteHelper.MakeSQLiteParameter("@UpdateTime", DbType.DateTime,bom.UpdateTime),
                    SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,bom.UniqueCode)
                    };
                    paramList.Add(parameter);
                    if (bom.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into BOM (VehicleModelCode,Material,UnitUsage,UpdateTime,UniqueCode) ")
                            .Append(@"values ( ")
                            .Append(@"@VehicleModelCode,@Material,@UnitUsage,@UpdateTime,@UniqueCode ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update BOM set VehicleModelCode=@VehicleModelCode,Material=@Material,UnitUsage=@UnitUsage,UpdateTime=@UpdateTime,UniqueCode=@UniqueCode ")
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
        /// 导入BOM到数据库
        /// </summary>
        /// <param name="bomList"></param>
        /// <param name="isCover">是否覆盖</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool ImportBOM(List<ExtractInventoryTool_BOM> bomList, bool isCover, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (bomList == null || bomList.Count == 0)
                    {
                        errorMessage = "BOM信息为空";
                        return false;
                    }
                    //先查询出所有的BOM唯一码
                    DataTable allMaterial = GetAllUniqueCode("BOM");
                    List<ExtractInventoryTool_BOM> allBOMList = new List<ExtractInventoryTool_BOM>();
                    foreach (DataRow row in allMaterial.Rows)
                    {
                        allBOMList.Add(new ExtractInventoryTool_BOM()
                        {
                            Oid = Int32.Parse(row["Oid"].ToString()),
                            UniqueCode = row["UniqueCode"].ToString()
                        });
                    }
                    StringBuilder insertStrbd = new StringBuilder();
                    StringBuilder updateStrbd = new StringBuilder();
                    List<SQLiteParameter[]> insertParamList = new List<SQLiteParameter[]>();
                    List<SQLiteParameter[]> updateParamList = new List<SQLiteParameter[]>();
                    foreach (var bom in bomList)
                    {
                        ExtractInventoryTool_BOM exitsBOM = allBOMList.FirstOrDefault(m => m.UniqueCode.Trim().Equals(bom.UniqueCode.Trim()));
                        if (exitsBOM != null && exitsBOM.Oid != 0)//导入数据如果表里有就更新，没有就新建
                        {
                            if (!isCover)
                            {
                                errorMessage = "存在重复数据，是否覆盖？";
                                return true;
                            }
                            bom.Oid = exitsBOM.Oid;
                        }
                        SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,bom.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@VehicleModelCode", DbType.String,bom.VehicleModelCode),
                            SQLiteHelper.MakeSQLiteParameter("@Material", DbType.Int32,bom.Material),
                            SQLiteHelper.MakeSQLiteParameter("@UnitUsage", DbType.Int32,bom.UnitUsage),
                            SQLiteHelper.MakeSQLiteParameter("@UpdateTime", DbType.DateTime,bom.UpdateTime),
                            SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,bom.UniqueCode)
                        };
                        if (exitsBOM != null && exitsBOM.Oid != 0)
                        {
                            updateParamList.Add(parameter);
                        }
                        else
                        {
                            insertParamList.Add(parameter);
                        }
                    }
                    //添加新数据
                    insertStrbd.Append(@"Insert into BOM (VehicleModelCode,Material,UnitUsage,UpdateTime,UniqueCode) ")
                        .Append(@"values ( ")
                        .Append(@"@VehicleModelCode,@Material,@UnitUsage,@UpdateTime,@UniqueCode ")
                        .Append(@")");
                    new SQLiteHelper().ExecuteNonQueryBatch(insertStrbd.ToString(), insertParamList);
                    updateStrbd.Append(@"Update BOM set VehicleModelCode=@VehicleModelCode,Material=@Material,UnitUsage=@UnitUsage,UpdateTime=@UpdateTime,UniqueCode=@UniqueCode ")
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

        /// <summary>
        /// 查询车型代码
        /// </summary>
        public DataTable QueryVehicleModelCode()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select distinct VehicleModelCode from BOM");
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
