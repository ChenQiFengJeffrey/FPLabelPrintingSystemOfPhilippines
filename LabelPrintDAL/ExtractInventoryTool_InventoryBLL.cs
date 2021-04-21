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
    public class ExtractInventoryTool_InventoryBLL:ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 查询库存
        /// </summary>
        public DataTable QueryInventory(string limit,string offset,out int totalCount)
        {
            lock (lockObj)
            {
                try
                {
                    totalCount = 0;
                    string countStr = " count(1) as TotalCount ";//用来计算总记录数
                    string selectStr = " it.*,mt.Name as MaterialName,mt.Code as MaterialCode,mt.Supplier as SupplierName,mt.SupplierCode as SupplierCode ";//查询语句需要查询的字段
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select {0} from Inventory it inner join Material mt on it.Material=mt.Oid");
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
        public int DeleteRecords(string tableName, List<string> ids, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    StringBuilder idStrbd = new StringBuilder();
                    foreach (string id in ids)
                    {
                        idStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    idStrbd.Remove(idStrbd.Length - 1, 1);
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



        public bool ImportInventory(List<ExtractInventoryTool_Inventory> inventoryList, bool isCover, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (inventoryList == null || inventoryList.Count == 0)
                    {
                        errorMessage = "库存信息为空";
                        return false;
                    }
                    //先查询出所有的BOM唯一码
                    DataTable allInventory = GetAllUniqueCode("Inventory", "Material");
                    List<ExtractInventoryTool_Inventory> allBOMList = new List<ExtractInventoryTool_Inventory>();
                    foreach (DataRow row in allInventory.Rows)
                    {
                        allBOMList.Add(new ExtractInventoryTool_Inventory()
                        {
                            Oid = Int32.Parse(row["Oid"].ToString()),
                            Material = Int32.Parse(row["Material"].ToString())
                        });
                    }
                    StringBuilder insertStrbd = new StringBuilder();
                    StringBuilder updateStrbd = new StringBuilder();
                    List<SQLiteParameter[]> insertParamList = new List<SQLiteParameter[]>();
                    List<SQLiteParameter[]> updateParamList = new List<SQLiteParameter[]>();
                    foreach (var inventory in inventoryList)
                    {
                        ExtractInventoryTool_Inventory exitsInventory = allBOMList.FirstOrDefault(m => m.Material==inventory.Material);
                        if (exitsInventory != null && exitsInventory.Oid != 0)//导入数据如果表里有就更新，没有就新建
                        {
                            if (!isCover)
                            {
                                errorMessage = "存在重复数据，是否覆盖？";
                                return true;
                            }
                            inventory.Oid = exitsInventory.Oid;
                        }
                        SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,inventory.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@Material", DbType.Int32,inventory.Material),
                            SQLiteHelper.MakeSQLiteParameter("@SysInventory", DbType.Int32,inventory.SysInventory),
                            SQLiteHelper.MakeSQLiteParameter("@Min", DbType.Int32,inventory.Min),
                            SQLiteHelper.MakeSQLiteParameter("@Max", DbType.Int32,inventory.Max),
                            SQLiteHelper.MakeSQLiteParameter("@HUB", DbType.Int32,inventory.HUB),
                            SQLiteHelper.MakeSQLiteParameter("@InTransit", DbType.Int32,inventory.InTransit),
                            SQLiteHelper.MakeSQLiteParameter("@Total", DbType.Int32,inventory.Total)
                        };
                        if (exitsInventory != null && exitsInventory.Oid != 0)
                        {
                            updateParamList.Add(parameter);
                        }
                        else
                        {
                            insertParamList.Add(parameter);
                        }
                    }
                    //添加新数据
                    insertStrbd.Append(@"Insert into Inventory (Material,SysInventory,Min,Max,HUB,InTransit,Total) ")
                        .Append(@"values ( ")
                        .Append(@"@Material,@SysInventory,@Min,@Max,@HUB,@InTransit,@Total ")
                        .Append(@")");
                    new SQLiteHelper().ExecuteNonQueryBatch(insertStrbd.ToString(), insertParamList);
                    updateStrbd.Append(@"Update Inventory set Material=@Material,SysInventory=@SysInventory,Min=@Min,Max=@Max,HUB=@HUB,InTransit=@InTransit,Total=@Total ")
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
