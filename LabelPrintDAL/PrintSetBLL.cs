﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrintDAL
{
    public class PrintSetBLL
    {
        public DataTable GetPrintSetByFPNum(string finishedProductNum)
        {
            DataTable dt = null;
            if (string.IsNullOrWhiteSpace(finishedProductNum))
                return null;
            StringBuilder queryStrbd = new StringBuilder();
            queryStrbd.Append("select * from PrintSet ")
                .Append("where FinishedProductNum=@FinishedProductNum");
            List<SQLiteParameter[]> paramList = new List<SQLiteParameter[]>();
            SQLiteParameter[] parameter = {
                    SQLiteHelper.MakeSQLiteParameter("@FinishedProductNum", DbType.String,finishedProductNum),
                    };
            paramList.Add(parameter);
            dt = new SQLiteHelper().ExecuteQuery(queryStrbd.ToString(), parameter);
            dt.TableName = "wcftable";
            return dt;
        }
    }
}