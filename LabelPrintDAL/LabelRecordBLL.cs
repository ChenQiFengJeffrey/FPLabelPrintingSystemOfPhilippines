using FPLabelData;
using FPLabelData.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrintDAL
{
    public class LabelRecordBLL
    {
        public void InsertLabelRecord(List<FinishedProductLabelDTO> dtoList)
        {
            try
            {
                if (dtoList == null || dtoList.Count == 0) return;
                StringBuilder noQueryStrbd = new StringBuilder();
                List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
                foreach (var dto in dtoList)
                {
                    SQLiteParameter[] parameter = {
                            SQLiteHelper.MakeSQLiteParameter("@Oid", DbType.Int32,dto.Oid),
                            SQLiteHelper.MakeSQLiteParameter("@ID", DbType.String,dto.ID),
                            SQLiteHelper.MakeSQLiteParameter("@A", DbType.Boolean,dto.A),
                            SQLiteHelper.MakeSQLiteParameter("@B", DbType.Boolean,dto.B),
                            SQLiteHelper.MakeSQLiteParameter("@C", DbType.Boolean,dto.C),
                            SQLiteHelper.MakeSQLiteParameter("@D", DbType.Boolean,dto.D),
                            SQLiteHelper.MakeSQLiteParameter("@E", DbType.Boolean,dto.E),
                            SQLiteHelper.MakeSQLiteParameter("@F", DbType.Boolean,dto.F),
                            SQLiteHelper.MakeSQLiteParameter("@G", DbType.Boolean,dto.G),
                            SQLiteHelper.MakeSQLiteParameter("@H", DbType.Boolean,dto.H),
                            SQLiteHelper.MakeSQLiteParameter("@I", DbType.Boolean,dto.I),
                            SQLiteHelper.MakeSQLiteParameter("@J", DbType.Boolean,dto.J),
                            SQLiteHelper.MakeSQLiteParameter("@K", DbType.Boolean,dto.K),
                            SQLiteHelper.MakeSQLiteParameter("@L", DbType.Boolean,dto.L),
                            SQLiteHelper.MakeSQLiteParameter("@M", DbType.Boolean,dto.M),
                            SQLiteHelper.MakeSQLiteParameter("@HOME", DbType.Boolean,dto.HOME),
                            SQLiteHelper.MakeSQLiteParameter("@SME", DbType.Boolean,dto.SME),
                            SQLiteHelper.MakeSQLiteParameter("@MSI", DbType.Boolean,dto.MSI),
                            SQLiteHelper.MakeSQLiteParameter("@FTTH", DbType.Boolean,dto.FTTH),
                            SQLiteHelper.MakeSQLiteParameter("@MSIVOICEONLY", DbType.Boolean,dto.MSIVOICEONLY),
                            SQLiteHelper.MakeSQLiteParameter("@COPPERDATAONLY", DbType.Boolean,dto.COPPERDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHDATAONLY", DbType.Boolean,dto.FTTHDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHNONWIFI", DbType.Boolean,dto.FTTHNONWIFI),
                            SQLiteHelper.MakeSQLiteParameter("@FTTHNONWIFIDATAONLY", DbType.Boolean,dto.FTTHNONWIFIDATAONLY),
                            SQLiteHelper.MakeSQLiteParameter("@ONU", DbType.String,dto.ONU),
                            SQLiteHelper.MakeSQLiteParameter("@VDSL", DbType.String,dto.VVDSL),
                            SQLiteHelper.MakeSQLiteParameter("@TELSET", DbType.String,dto.TELSET),
                            SQLiteHelper.MakeSQLiteParameter("@BIZBOX", DbType.String,dto.BIZBOX),
                            SQLiteHelper.MakeSQLiteParameter("@Barcode", DbType.String,dto.Barcode),
                            SQLiteHelper.MakeSQLiteParameter("@GoodList", DbType.String,JsonConvert.SerializeObject(dto.GoodList)),
                            SQLiteHelper.MakeSQLiteParameter("@WorkStation", DbType.String,dto.WorkStation),
                            SQLiteHelper.MakeSQLiteParameter("@CreateTime", DbType.DateTime,DateTime.Now),
                            SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.String,dto.FinishedProductNum),
                            SQLiteHelper.MakeSQLiteParameter("@RoNumber", DbType.String,dto.RoNumber)
                            };
                    paramList.Add(parameter);
                    //添加新数据
                }
                noQueryStrbd.Append(@"Insert into LabelRecord (ID,A,B,C,D,E,F,G,H,I,J,K,L,M,HOME,SME,MSI,FTTH,MSIVOICEONLY,COPPERDATAONLY,FTTHDATAONLY,FTTHNONWIFI,FTTHNONWIFIDATAONLY,ONU,VDSL,TELSET,BIZBOX,Barcode,GoodList,CreateTime,WorkStation,FinishedProductNum,RoNumber) ")
                    .Append(@"values ( ")
                    .Append(@"@ID,@A,@B,@C,@D,@E,@F,@G,@H,@I,@J,@K,@L,@M,@HOME,@SME,@MSI,@FTTH,@MSIVOICEONLY,@COPPERDATAONLY,@FTTHDATAONLY,@FTTHNONWIFI,@FTTHNONWIFIDATAONLY,@ONU,@VDSL,@TELSET,@BIZBOX,@Barcode,@GoodList,@CreateTime,@WorkStation,@FinishedProductNum,@RoNumber ")
                    .Append(@")");
                new SQLiteHelper().ExecuteNonQueryBatch(noQueryStrbd.ToString(), paramList);
                return;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("InsertLabelRecord", ex);
            }
        }
    }
}
