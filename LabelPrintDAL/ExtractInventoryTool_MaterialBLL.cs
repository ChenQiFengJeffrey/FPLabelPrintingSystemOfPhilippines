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
    public class ExtractInventoryTool_MaterialBLL : ExtractInventoryTool_BaseBLL
    {
        /// <summary>
        /// 通过客户ID查询物料
        /// </summary>
        /// <returns></returns>
        public DataTable QueryMaterialByClientID(string clientID)
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select * from Material where 1=1 ");
                    if (!string.IsNullOrEmpty(clientID))
                    {
                        queryStrbd.Append(" and Client = ").Append(clientID);
                    }
                    return new SQLiteHelper().ExecuteQuery(queryStrbd.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 添加/更新物料配置
        /// </summary>
        /// <param name="material"></param>
        public void InsertOrUpdateMaterial(ExtractInventoryTool_Material material, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (material == null)
                    {
                        MessageBox.Show("物料信息为空", "Error");
                    }
                    string oid = material.Oid == 0 ? string.Empty : material.Oid.ToString();
                    if (IsExistsUniqueCode(material.UniqueCode, "Material", oid))
                    {
                        errorMessage = "该物料已存在，无法添加相同物料";
                        return ;
                    }
                    StringBuilder noQueryStrbd = new StringBuilder();
                    List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                    SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,material.Oid),
                    SQLiteHelper.MakeSQLiteParameter("@Client", DbType.Int32,material.Client),
                    SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String,material.Name),
                    SQLiteHelper.MakeSQLiteParameter("@Code", DbType.String,material.Code),
                    SQLiteHelper.MakeSQLiteParameter("@Supplier", DbType.String,material.Supplier),
                    SQLiteHelper.MakeSQLiteParameter("@SupplierCode", DbType.String,material.SupplierCode),
                    SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,material.UniqueCode)
                    };
                    paramList.Add(parameter);
                    if (material.Oid == 0)
                    {
                        //添加新数据
                        noQueryStrbd.Append(@"Insert into Material (Client,Name,Code,Supplier,SupplierCode,UniqueCode) ")
                            .Append(@"values ( ")
                            .Append(@"@Client,@Name,@Code,@Supplier,@SupplierCode,@UniqueCode ")
                            .Append(@")");
                    }
                    else
                    {
                        //更新数据
                        noQueryStrbd.Append(@"Update Material set Client=@Client,Name=@Name,Code=@Code,Supplier=@Supplier,SupplierCode=@SupplierCode,UniqueCode=@UniqueCode ")
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
        /// 导入物料备案
        /// </summary>
        /// <param name="materialList">物料信息</param>
        /// <param name="isCover">是否覆盖已有数据</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        /// <remarks>
        /// Meritar_Jeffrey		2021/04/14 16:31:13
        /// 功能 : 
        /// </remarks>
        public bool ImportMaterial(List<ExtractInventoryTool_Material> materialList, bool isCover, out string errorMessage)
        {
            lock (lockObj)
            {
                try
                {
                    errorMessage = string.Empty;
                    if (materialList == null || materialList.Count == 0)
                    {
                        errorMessage = "物料信息为空";
                        return false;
                    }
                    //先查询出所有的物料唯一码
                    DataTable allMaterial = GetAllUniqueCode("Material");
                    List<ExtractInventoryTool_Material> allMaterialList = new List<ExtractInventoryTool_Material>();
                    foreach (DataRow row in allMaterial.Rows)
                    {
                        allMaterialList.Add(new ExtractInventoryTool_Material() {
                            Oid=Int32.Parse(row["Oid"].ToString()),
                            UniqueCode=row["UniqueCode"].ToString()
                        });
                    }
                    StringBuilder insertStrbd = new StringBuilder();
                    StringBuilder updateStrbd = new StringBuilder();
                    List<SQLiteParameter[]> insertParamList = new List<SQLiteParameter[]>();
                    List<SQLiteParameter[]> updateParamList = new List<SQLiteParameter[]>();
                    foreach (var material in materialList)
                    {
                        ExtractInventoryTool_Material exitsMaterial= allMaterialList.FirstOrDefault(m => m.UniqueCode.Trim().Equals(material.UniqueCode.Trim()));
                        if (exitsMaterial != null && exitsMaterial.Oid != 0)//导入数据如果表里有就更新，没有就新建
                        {
                            if (!isCover)
                            {
                                errorMessage = "存在重复数据，是否覆盖？";
                                return true;
                            }
                            material.Oid = exitsMaterial.Oid;
                        }
                        SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,material.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@Client", DbType.Int32,material.Client),
                            SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String,material.Name),
                            SQLiteHelper.MakeSQLiteParameter("@Code", DbType.String,material.Code),
                            SQLiteHelper.MakeSQLiteParameter("@Supplier", DbType.String,material.Supplier),
                            SQLiteHelper.MakeSQLiteParameter("@SupplierCode", DbType.String,material.SupplierCode),
                            SQLiteHelper.MakeSQLiteParameter("@UniqueCode", DbType.String,material.UniqueCode)
                        };
                        if (exitsMaterial != null && exitsMaterial.Oid != 0)
                        {
                            updateParamList.Add(parameter);
                        }
                        else {
                            insertParamList.Add(parameter);
                        }
                        
                    }
                    //添加新数据
                    insertStrbd.Append(@"Insert into Material (Client,Name,Code,Supplier,SupplierCode,UniqueCode) ")
                        .Append(@"values ( ")
                        .Append(@"@Client,@Name,@Code,@Supplier,@SupplierCode,@UniqueCode ")
                        .Append(@")");
                    new SQLiteHelper().ExecuteNonQueryBatch(insertStrbd.ToString(), insertParamList);
                    updateStrbd.Append(@"Update Material set Client=@Client,Name=@Name,Code=@Code,Supplier=@Supplier,SupplierCode=@SupplierCode,UniqueCode=@UniqueCode ")
                            .Append(@" WHERE Oid=@Oid");
                    new SQLiteHelper().ExecuteNonQueryBatch(updateStrbd.ToString(), updateParamList);
                    return true;
                }
                catch (Exception ex)
                {
                    //唯一码重复，须提示
                    //debug();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 删除物料
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
                    //判断是否有该客户的物料，如果有，则不能删除
                    StringBuilder hasBOMStrbd = new StringBuilder();
                    StringBuilder idStrbd = new StringBuilder();
                    foreach (string id in ids)
                    {
                        idStrbd.Append("'")
                            .Append(id)
                            .Append("',");
                    }
                    idStrbd.Remove(idStrbd.Length - 1, 1);
                    hasBOMStrbd.Append("select count(*) from BOM where Material in ( ")
                                                .Append(idStrbd).Append(" ) ");
                    var hasBOM = new SQLiteHelper().ExecuteScalar(hasBOMStrbd.ToString());
                    if (Convert.ToInt32(hasBOM) > 0)
                    {
                        errorMessage = "存在与待删除物料相关联的BOM，不能删除";
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

        /// <summary>
        /// Material和Client联表查询
        /// </summary>
        /// <returns></returns>
        public DataTable QueryMaterialExtension()
        {
            lock (lockObj)
            {
                try
                {
                    StringBuilder queryStrbd = new StringBuilder();
                    queryStrbd.Append("select mt.*,cl.Name as ClientName,cl.UniqueCode as ClientCode from Material mt inner join Client cl on mt.Client = cl.Oid ");
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
