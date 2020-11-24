using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace LabelPrintDAL
{
    public class SQLiteHelper
    {
        public static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        /// <summary>
        /// 创建一个数据库文件。如果存在同名数据库文件，则会覆盖。
        /// </summary>
        /// <param name="dbName">数据库文件名。为null或空串时不创建。</param>
        public static void CreateDB(string dbName)
        {
            if (!string.IsNullOrEmpty(dbName))
            {
                try { SQLiteConnection.CreateFile(dbName); }
                catch (Exception) { throw; }
            }
        }

        /// <summary> 
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。 
        /// </summary> 
        /// <param name="sql">要执行的增删改的SQL语句。</param> 
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            int affectedRows = 0;
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteTransaction tran = conn.BeginTransaction())
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            try
                            {

                                cmd.CommandText = sql;
                                if (parameters.Length != 0)
                                {
                                    cmd.Parameters.AddRange(parameters);
                                }
                                affectedRows = cmd.ExecuteNonQuery();
                                tran.Commit();
                            }
                            catch (Exception) { tran.Rollback(); throw; }
                        }
                    }
                }
                catch { throw; }
                finally
                {
                    conn.Clone();
                }
            }
            return affectedRows;
        }

        /// <summary>
        /// 批量处理数据操作语句。
        /// </summary>
        /// <param name="sql">要执行的增删改的SQL语句</param> 
        /// <param name="list">执行增删改语句所需要的参数，SQL语句中的字段顺序为准</param> 
        public void ExecuteNonQueryBatch(string sql, List<SQLiteParameter[]> list)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SQLiteTransaction tran = conn.BeginTransaction())
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            try
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    cmd.CommandText = sql;
                                    cmd.Parameters.AddRange(list[i]);
                                    cmd.ExecuteNonQuery();
                                }
                                tran.Commit();
                            }
                            catch (Exception) { tran.Rollback(); throw; }
                        }
                    }
                }
                catch { throw; }
                finally
                {
                    conn.Clone();
                }
            }
        }

        /// <summary>
        /// 执行查询语句，并返回第一个结果。
        /// </summary>
        /// <param name="sql">查询语句。</param>
        /// <returns>查询结果。</returns>
        public object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = sql;
                        if (parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteScalar();
                    }
                    catch (Exception) { throw; }
                }
            }
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable。 
        /// </summary> 
        /// <param name="sql">要执行的查询语句。</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param> 
        /// <returns></returns> 
        public DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (parameters.Length != 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable data = new DataTable();
                    try { adapter.Fill(data); }
                    catch (Exception) { throw; }
                    finally
                    {
                        connection.Clone();
                    }
                    return data;
                }
            }
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例。 
        /// </summary> 
        /// <param name="sql">要执行的查询语句。</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param> 
        /// <returns></returns> 
        public SQLiteDataReader ExecuteReader(string sql, params SQLiteParameter[] parameters)
        {
            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            try
            {
                if (parameters.Length != 0)
                {
                    command.Parameters.AddRange(parameters);
                }
                connection.Open();
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception) { throw; }
        }

        /// <summary> 
        /// 查询数据库中的所有数据类型信息。
        /// </summary> 
        /// <returns></returns> 
        public DataTable GetSchema()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    return connection.GetSchema("TABLES");
                }
                catch (Exception) { throw; }
            }
        }

        /// <summary>
        /// 放回一个SQLiteParameter
        /// </summary>
        /// <param name="name">参数名字</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="value">参数值</param>
        /// <returns>SQLiteParameter的值</returns>
        public static SQLiteParameter MakeSQLiteParameter(string name, DbType type, int size, object value)
        {
            SQLiteParameter parm = new SQLiteParameter(name, type, size);
            parm.Value = value;
            return parm;
        }

        public static SQLiteParameter MakeSQLiteParameter(string name, DbType type, object value)
        {
            SQLiteParameter parm = new SQLiteParameter(name, type);
            parm.Value = value;
            return parm;
        }
    }
}
