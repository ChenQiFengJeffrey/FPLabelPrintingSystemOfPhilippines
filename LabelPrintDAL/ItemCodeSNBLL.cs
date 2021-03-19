using FPLabelData.DTO;
using Pivots.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrintDAL
{
    public class ItemCodeSNBLL
    {
        public string UploadItemCodeAndSNList(ItemCodeSN_UploadDTO value)
        {
            if (value == null || value.SNList == null || value.SNList.Count <= 0)
                return "SN List is null !";
            if (string.IsNullOrEmpty(value.RoNumber))
                return "RoNumber is null !";
            if (string.IsNullOrEmpty(value.ItemCode))
                return "Raw Material Label isn't scanned !";
            try
            {
                string roNumber = value.RoNumber;
                string itemCode = value.ItemCode;
                string itemName = string.Empty;
                string finishedProductNum = value.FinishedProductNum;
                List<ItemCodeSNDTO> snList = value.SNList;
                #region 查询原材料备案
                StringBuilder rawMaterialReaderStrbd = new StringBuilder();
                rawMaterialReaderStrbd.Append(@"select m.Code from Material m where m.Name = @Name ");
                SQLiteParameter[] readParameter = { SQLiteHelper.MakeSQLiteParameter("@Name", DbType.String, itemCode) };
                object rawMaterialObject = new SQLiteHelper().ExecuteScalar(rawMaterialReaderStrbd.ToString(), readParameter);
                if (rawMaterialObject == null)
                    return "Raw Material Name is empty !";
                itemName = rawMaterialObject.ToString();
                #endregion
                #region 插入原材料SN列表
                StringBuilder noQueryStrbd = new StringBuilder();
                noQueryStrbd.Append(@"insert into ItemCodeSN (ItemCode,ItemName,FinishedProductNum,SerivalNum,RoNumber) ")
                    .Append(@"values ( ")
                    .Append(@"@ItemCode,@ItemName,@FinishedProductNum,@SerivalNum,@RoNumber ")
                    .Append(@") ");
                List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                foreach (var dto in snList)
                {
                    SQLiteParameter[] parameter = {
                        SQLiteHelper.MakeSQLiteParameter("@ItemCode", DbType.String,dto.ItemCode),
                        SQLiteHelper.MakeSQLiteParameter("@ItemName", DbType.String,itemName),
                        SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.String,finishedProductNum),
                        SQLiteHelper.MakeSQLiteParameter("@SerivalNum", DbType.String,dto.SerivalNum),
                        SQLiteHelper.MakeSQLiteParameter("@RoNumber", DbType.String,roNumber)
                    };
                    paramList.Add(parameter);
                }
                new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList); 
                #endregion
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("UploadItemCodeAndSNList", ex);
                return ex.Message;
            }
        }
    }
}
