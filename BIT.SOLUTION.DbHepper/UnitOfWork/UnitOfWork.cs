using BIT.SOLUTION.Common;
using BIT.SOLUTION.Data;
using BIT.SOLUTION.Lib;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BIT.SOLUTION.DbHepper
{
    public class UnitOfWork : IUnitOfWork
    {
        private static Dictionary<string, List<Columns>> disSchemas = new Dictionary<string, List<Columns>>();
        private MySqlDataBase msqlDB = null;
        private string _connectionString = string.Empty;
        public string ConnectionString {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
        private MySqlConnection connection = null;
        private MySqlTransaction transaction  = null;
        public MySqlDataBase DB {
            get
            {
                if(msqlDB == null)
                {
                    if(string.IsNullOrEmpty(this._connectionString))
                    {
                        this._connectionString = @"server=103.97.124.229;user=luat;database=Employee_db;password=m-5J1stYuABvhkCO;";// ham get connection
                    }
                    msqlDB = new MySqlDataBase(this._connectionString);
                }
                return msqlDB;
            }
        }
        public void CreateConn()
        {
            if (this.connection == null)
            {
                this.connection = DB.CreateConnection();
            }
        }

        public void CreateSQL(string commandCode)
        {
            if (string.IsNullOrEmpty(this._connectionString))
            {
                this._connectionString = @"server=103.97.124.229;user=luat;database=Employee_db;password=m-5J1stYuABvhkCO;"; // Hàm lấy connection
            }
        }
        public MySqlConnection DbConnection
        {
            get
            {
                if(this.connection == null)
                {
                    this.connection = DB.CreateConnection();
                }
                return this.connection;
            }
        }

      

        public void BeginTransaction()
        {
            if(this.transaction ==null)
            {
                CreateConn();
                OpenConn();
                this.transaction = this.connection.BeginTransaction();
            }    
        }
        public void Commit()
        {
           if(transaction == null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }    
        }
        public void RollBack()
        {
            if (transaction == null)
            {
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
            }
        }
        
        public bool ExecuteNoneQueryCmd(MySqlCommand cmd)
        {
            bool success = true;
            if(transaction != null)
            {
                success = this.DB.ExecuteNonQuery(cmd,transaction) > 0;
            }
            else
            {
                success = this.DB.ExecuteNonQuery(cmd) > 0;
            }
            return success;
        }

        public MySqlDataReader ExecuteRenderCmd(MySqlCommand cmd)
        {
            bool success = true;
            if (transaction != null)
            {
                return  this.DB.ExcuteRender(cmd, transaction);
            }
            else
            {
                return  this.DB.ExcuteRender(cmd);
            }
        }

        public object ExecuteScalar(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                object rs = null;
                cmd = this.CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsDictionary(param, cmd, ref indexOutputParams);
                if(transaction != null)
                {
                    rs = this.DB.ExcuteScalar(cmd, transaction);
                }
                else
                {
                    rs = this.DB.ExcuteScalar(cmd);
                }
                SetOutPutParamsDictionary(param, cmd, indexOutputParams);
                return rs;
            }
            catch(MySqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public object ExecuteScalar(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                object rs = null;
                cmd = this.CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsObject(param, cmd, ref indexOutputParams);
                if (transaction != null)
                {
                    rs = this.DB.ExcuteScalar(cmd, transaction);
                }
                else
                {
                    rs = this.DB.ExcuteScalar(cmd);
                }
                SetOutPutParamsObject(param, cmd, indexOutputParams);
                return rs;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public object ExecuteScalarCmd(MySqlCommand cmd)
        {
           if(transaction != null)
            {
                return this.DB.ExcuteScalar(cmd, transaction);
            }
           else
            {
                return this.DB.ExcuteScalar(cmd);
            }
        }

        public object GetFistOrDefaultID(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                cmd = CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsDictionary(param, cmd, ref indexOutputParams);
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                SetOutPutParamsDictionary(param, cmd, indexOutputParams);
                return this.ExecuteScalarCmd(cmd);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }
        public DataTable GetDataTable(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                cmd = CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsDictionary(param, cmd, ref indexOutputParams);
                DataTable dt = new DataTable();
                if(transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                using(MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                SetOutPutParamsDictionary(param, cmd, indexOutputParams);
                return dt;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public DataTable GetDataTable(string query, object param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                cmd = CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsObject(param, cmd, ref indexOutputParams);
                DataTable dt = new DataTable();
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                SetOutPutParamsObject(param, cmd, indexOutputParams);
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public MySqlCommand GetStoreProcCommand(string storeName, object[] parameterValue)
        {
           if(connection == null)
            {
                CreateConn();
            }
            OpenConn();
            MySqlCommand cmd = this.DB.GetStoreProcCommand(storeName, parameterValue);
            return cmd;
        }
        public MySqlCommand GetCommandText(string cm, object[] parameterValue)
        {
            if (connection == null)
            {
                CreateConn();
            }
            OpenConn();
            MySqlCommand cmd = this.DB.GetStoreProcCommand(cm, parameterValue);
            return cmd;
        }

        public void OpenConn()
        {
            if(this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

      
        public void AddWithValue(MySqlCommand cmd, string paramaterName, object value)
        {
            if(value != null)
            {
                cmd.Parameters.AddWithValue($"@{paramaterName}", value);
            }
            else
            {
                cmd.Parameters.AddWithValue($"@{paramaterName}",DBNull.Value);
            }
        }
       
        public void CloseConn()
        {
           if(this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }



        public MySqlCommand CreateCommand()
        {
            CreateConn();
            OpenConn();
            MySqlCommand cmd = this.connection.CreateCommand();
            return cmd;
        }


        public void Dispose()
        {
           
            if(transaction != null)
            {
                RollBack();
            }
            this.DisposeConn();
        }

        public void DisposeConn()
        {
            if(connection != null)
            {
                try
                {
                    if(this.connection.State !=  ConnectionState.Closed)
                    {
                        this.connection.Close();
                    }
                    this.connection.Dispose();
                }
                catch(Exception)
                {

                }
                finally
                {
                    connection = null;
                }
            }
        }

        public int Execute(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                int rs = -1;
                cmd = this.CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsDictionary(param, cmd, ref indexOutputParams);
                if (transaction != null)
                {
                    rs = this.DB.ExecuteNonQuery(cmd, transaction);
                }
                else
                {
                    rs = this.DB.ExecuteNonQuery(cmd);
                }
                SetOutPutParamsDictionary(param, cmd, indexOutputParams);
                return rs;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public int Execute(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlCommand cmd = null;
            try
            {
                int rs = -1;
                cmd = this.CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsObject(param, cmd, ref indexOutputParams);
                if (transaction != null)
                {
                    rs = this.DB.ExecuteNonQuery(cmd, transaction);
                }
                else
                {
                    rs = this.DB.ExecuteNonQuery(cmd);
                }
                SetOutPutParamsObject(param, cmd, indexOutputParams);
                return rs;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(null, cmd);
            }
        }

        public List<Dictionary<string, object>> QueryListDictionary(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            List<Dictionary<string, object>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListDictionary();
            }, param, commandType, timeOut);
            return rs;
        }

        public Tuple<List<T1>, List<T2>> QueryMultiResult<T1, T2>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            Tuple<List<T1>, List<T2>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListObject<T1>();
                if(rd.NextResult())
                {
                    var rsT2 = rd.ToListObject<T2>();
                    rs = new Tuple<List<T1>, List<T2>>(rsT1, rsT2);
                }
                else
                {
                    rs = new Tuple<List<T1>, List<T2>>(rsT1, null);
                }
            },param,commandType,timeOut);
            return rs;
        }

        public Tuple<List<T1>, List<T2>> QueryMultiResult<T1, T2>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            Tuple<List<T1>, List<T2>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListObject<T1>();
                if (rd.NextResult())
                {
                    var rsT2 = rd.ToListObject<T2>();
                    rs = new Tuple<List<T1>, List<T2>>(rsT1, rsT2);
                }
                else
                {
                    rs = new Tuple<List<T1>, List<T2>>(rsT1, null);
                }
            }, param, commandType, timeOut);
            return rs;
        }

        public Tuple<List<T1>, List<T2>, List<T3>> QueryMultiResult<T1, T2, T3>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            Tuple<List<T1>, List<T2>, List<T3>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListObject<T1>();
                if (rd.NextResult())
                {
                    var rsT2 = rd.ToListObject<T2>();
                   if(rd.NextResult())
                   {
                        var rsT3 = rd.ToListObject<T3>();
                        rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, rsT2, rsT3);
                   }
                   else
                    {
                        rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, rsT2, null);
                    }
                }
                else
                {
                    rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, null, null);
                }
            }, param, commandType, timeOut);
            return rs;
        }

        public Tuple<List<T1>, List<T2>, List<T3>> QueryMultiResult<T1, T2, T3>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            Tuple<List<T1>, List<T2>, List<T3>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListObject<T1>();
                if (rd.NextResult())
                {
                    var rsT2 = rd.ToListObject<T2>();
                    if (rd.NextResult())
                    {
                        var rsT3 = rd.ToListObject<T3>();
                        rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, rsT2, rsT3);
                    }
                    else
                    {
                        rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, rsT2, null);
                    }
                }
                else
                {
                    rs = new Tuple<List<T1>, List<T2>, List<T3>>(rsT1, null, null);
                }
            }, param, commandType, timeOut);
            return rs;
        }

        public Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> QueryMultiResultDictionnary(string query, Type type, Dictionary<string, object> param, CommandType? commandType = CommandType.Text)
        {
            Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListDictionary();
                if(rd.NextResult())
                {
                    var rsT12 = rd.ToListDictionary();
                    rs = new Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>>(rsT1, rsT12);
                }
                else
                {
                    rs = new Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>>(rsT1, null);
                }
               
            }, param, commandType);
            return rs;
        }

        public Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> QueryMultiResultDictionnary(string query, Type type, object param = null, CommandType? commandType = CommandType.Text)
        {
            Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                var rsT1 = rd.ToListDictionary();
                if (rd.NextResult())
                {
                    var rsT12 = rd.ToListDictionary();
                    rs = new Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>>(rsT1, rsT12);
                }
                else
                {
                    rs = new Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>>(rsT1, null);
                }

            }, param, commandType);
            return rs;
        }

        /// <summary>
        /// Lưu data theo entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
       

        public bool SaveEntity(object entity, int entityState = 0, string tableName = "", params string[] columsIgnore)
        {
            if (entity == null)
            {
                return false;
            }
            if (entityState <= 0)
            {
                entityState = entity.GetValue<int>("MTEntitySate");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = entity.GetValue<string>("TableName");
            }
            string keyCached = $"MYSQLDatabaseInfo_{ _connectionString}_{tableName}".MD5Hash();
            List<Columns> columns = new List<Columns>();
            if (!disSchemas.TryGetValue(keyCached, out columns))
            {
                string queryCOLUMN_INFO = $@"SELECT COLUMN_NAME as FieldName,DATA_TYPE as DataType,
                                         CHARACTER_MAXIMUM_LENGTH as MaxLength, CASE WHEN COLUMN_KEY='PRI' THEN 1 ELSE 0 END AS IsIdentity,
                                         CASE WHEN IS_NULLABLE='YES' THEN 1 ELSE 0 END as IsNullable, Extra FROM Informatin_schema.Columns 
                                         WHERE Table_Name = @Table_name AND Table_Schema =database() ORDER BY Column_Name";
                columns = this.Query<Columns>(queryCOLUMN_INFO, new { Table_Name = tableName });
                if (columns != null && columns.Count > 0)
                {
                    if (!disSchemas.ContainsKey(keyCached))
                    {
                        lock (disSchemas)
                        {
                            disSchemas.Add(keyCached, columns);
                        }
                    }
                }
            }
            Columns columnsPK = columns.Find(m => m.IsIdentity);
            bool autoIncrenment = "auto_increment".Equals(columnsPK.Extra, StringComparison.OrdinalIgnoreCase);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string query = string.Empty;
            string primaryKeyName = columnsPK.FieldName;
            bool success = false;
            switch (entityState)
            {
                case 1:
                    StringBuilder builderColumns = new StringBuilder();
                    StringBuilder builderValues = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        var pInfo = entity.GetType().GetProperty(fieldName);
                        if (pInfo == null)
                        {
                            continue;
                        }
                        builderColumns.Append($",`{fieldName}`");
                        builderValues.Append($",@{fieldName}");
                        ProcessDataBeforeSave(values, entity, c);
                    }
                    if (autoIncrenment)
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)}); SELECT LAST_INSERT_ID()";
                        TraceLogSaveEntity(query, values);
                        object primaryKeyValue = this.ExecuteScalar(query, values);
                        if (primaryKeyValue != null)
                        {
                            entity.SetValue(primaryKeyName, primaryKeyValue);
                            success = true;
                        }

                    }
                    else
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)});";
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values) > 0;
                    }
                    break;
                case 2:
                    string keyCachedUpdate = $"{keyCached}_query_update";
                    StringBuilder builderUpdate = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase))
                            || c.FieldName.Equals(CommonKey.btsCreateDate)
                            || c.FieldName.Equals(CommonKey.btsCreateBy))
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        var pInfo = entity.GetType().GetProperty(fieldName);
                        if (pInfo == null)
                        {
                            continue;
                        }
                        ProcessDataBeforeSave(values, entity, c);
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        builderUpdate.Append($", `{fieldName}`= @{fieldName}");
                    }
                    query = $"UPDATEE `{tableName}` SET {builderUpdate.ToString().Substring(1)} WHERE `{primaryKeyName}` = @{primaryKeyName}";
                    TraceLogSaveEntity(query, values);
                    success = this.Execute(query, values) > 0;
                    break;
                case 3:
                        query = $"DELETE FROM `{tableName}` WHERE `{primaryKeyName}` = @{primaryKeyName}";
                        values.AddOrUpdateDictionary(primaryKeyName, entity.GetValue<object>(primaryKeyName));
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values) > 0;
                    break;
            }
            return success;
        }
        /// <summary>
        /// ghi lgi
        /// </summary>
        /// <param name="query"></param>
        /// <param name="values"></param>
        private void TraceLogSaveEntity(string query, Dictionary<string,object> values)
        {
            /// chưa có hàm ghi log
        }
        /// <summary>
        /// Xu ly du lieu truoc khi luu db
        /// </summary>
        /// <param name="values"></param>
        /// <param name="entity"></param>
        /// <param name="col"></param>
        private void ProcessDataBeforeSave(Dictionary<string,object> values, object entity, Columns col)
        {
            if(string.Compare("varchar",col.DataType)==0 && col.MaxLength >0)
            {
                string text = entity.GetValue<string>(col.FieldName);
                if(!string.IsNullOrEmpty(text) && text.Length >col.MaxLength)
                {
                    text = text.Trim().Substring(0, (int)col.MaxLength);
                }
                values.Add(col.FieldName, text);
            }
            else
            {
                values.Add(col.FieldName, entity.GetValue<object>(col.FieldName));
            }
        }
        /// <summary>
        /// Lưu data Entity vào db
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        public bool SaveEntity(Dictionary<string, object> entity, int entityState = 0, string tableName = "", params string[] columsIgnore)
        {
            if (entity == null)
            {
                return false;
            }
            string keyCached = $"MYSQLDatabaseInfo_{ _connectionString}_{tableName}".MD5Hash();
            List<Columns> columns = new List<Columns>();
            if (!disSchemas.TryGetValue(keyCached, out columns))
            {
                string queryCOLUMN_INFO = $@"SELECT COLUMN_NAME as FieldName,DATA_TYPE as DataType,
                                         CHARACTER_MAXIMUM_LENGTH as MaxLength, CASE WHEN COLUMN_KEY='PRI' THEN 1 ELSE 0 END AS IsIdentity,
                                         CASE WHEN IS_NULLABLE='YES' THEN 1 ELSE 0 END as IsNullable, Extra FROM Informatin_schema.Columns 
                                         WHERE Table_Name = @Table_name AND Table_Schema =database() ORDER BY Column_Name";
                columns = this.Query<Columns>(queryCOLUMN_INFO, new { Table_Name = tableName });
                if (columns != null && columns.Count > 0)
                {
                    if (!disSchemas.ContainsKey(keyCached))
                    {
                        lock (disSchemas)
                        {
                            disSchemas.Add(keyCached, columns);
                        }
                    }
                }
            }
            Columns columnsPK = columns.Find(m => m.IsIdentity);
            bool autoIncrenment = "auto_increment".Equals(columnsPK.Extra, StringComparison.OrdinalIgnoreCase);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string query = string.Empty;
            string primaryKeyName = columnsPK.FieldName;
            bool success = false;
            switch (entityState)
            {
                case 1:
                    StringBuilder builderColumns = new StringBuilder();
                    StringBuilder builderValues = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        if(!entity.ContainsKey(fieldName))
                        {
                            continue;
                        }
                        builderColumns.Append($",`{fieldName}`");
                        builderValues.Append($",@{fieldName}");
                        ProcessDataBeforeSave(values, entity, c);
                    }
                    if (autoIncrenment)
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)}); SELECT LAST_INSERT_ID()";
                        TraceLogSaveEntity(query, values);
                        object primaryKeyValue = this.ExecuteScalar(query, values);
                        if (primaryKeyValue != null)
                        {
                            entity.SetValue(primaryKeyName, primaryKeyValue);
                            success = true;
                        }

                    }
                    else
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)});";
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values) > 0;
                    }
                    break;
                case 2:
                    string keyCachedUpdate = $"{keyCached}_query_update";
                    StringBuilder builderUpdate = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase))
                            || c.FieldName.Equals(CommonKey.btsCreateDate)
                            || c.FieldName.Equals(CommonKey.btsCreateBy))
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        if (!entity.ContainsKey(fieldName))
                        {
                            continue;
                        }
                        ProcessDataBeforeSave(values, entity, c);
                        if(c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        builderUpdate.Append($", `{fieldName}`= @{fieldName}");
                    }
                    query = $"UPDATEE `{tableName}` SET {builderUpdate.ToString().Substring(1)} WHERE `{primaryKeyName}` = @{primaryKeyName}";
                    TraceLogSaveEntity(query, values);
                    success = this.Execute(query, values) >0;
                    break;
                case 3:
                    if(entity.ContainsKey(primaryKeyName))
                    {
                        query = $"DELETE FROM `{tableName}` WHERE `{primaryKeyName}` = @{primaryKeyName}";
                        values.AddOrUpdateDictionary(primaryKeyName, entity[primaryKeyName]);
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values) >0;
                    }
                    break;
            }
            return success;
        }
        /// <summary>
        /// Lưu và lấy ra ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        public long SaveEntityReturnID(Dictionary<string, object> entity, int entityState, string tableName, params string[] columsIgnore)
        {
            if (entity == null)
            {
                return 0;
            }
            string keyCached = $"MYSQLDatabaseInfo_{ _connectionString}_{tableName}".MD5Hash();
            List<Columns> columns = new List<Columns>();
            if (!disSchemas.TryGetValue(keyCached, out columns))
            {
                string queryCOLUMN_INFO = $@"SELECT COLUMN_NAME as FieldName,DATA_TYPE as DataType,
                                         CHARACTER_MAXIMUM_LENGTH as MaxLength, CASE WHEN COLUMN_KEY='PRI' THEN 1 ELSE 0 END AS IsIdentity,
                                         CASE WHEN IS_NULLABLE='YES' THEN 1 ELSE 0 END as IsNullable, Extra FROM Informatin_schema.Columns 
                                         WHERE Table_Name = @Table_name AND Table_Schema =database() ORDER BY Column_Name";
                columns = this.Query<Columns>(queryCOLUMN_INFO, new { Table_Name = tableName });
                if (columns != null && columns.Count > 0)
                {
                    if (!disSchemas.ContainsKey(keyCached))
                    {
                        lock (disSchemas)
                        {
                            disSchemas.Add(keyCached, columns);
                        }
                    }
                }
            }
            Columns columnsPK = columns.Find(m => m.IsIdentity);
            bool autoIncrenment = "auto_increment".Equals(columnsPK.Extra, StringComparison.OrdinalIgnoreCase);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string query = string.Empty;
            string primaryKeyName = columnsPK.FieldName;
            long success = 0;
            switch (entityState)
            {
                case 1:
                    StringBuilder builderColumns = new StringBuilder();
                    StringBuilder builderValues = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        if (!entity.ContainsKey(fieldName))
                        {
                            continue;
                        }
                        builderColumns.Append($",`{fieldName}`");
                        builderValues.Append($",@{fieldName}");
                        ProcessDataBeforeSave(values, entity, c);
                    }
                    if (autoIncrenment)
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)}); SELECT LAST_INSERT_ID()";
                        TraceLogSaveEntity(query, values);
                        object primaryKeyValue = this.ExecuteScalar(query, values);
                        if (primaryKeyValue != null)
                        {
                            entity.AddOrUpdateDictionary(primaryKeyName, primaryKeyValue);
                            success = Convert.ToInt64(primaryKeyValue);
                        }

                    }
                    else
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)});";
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values);
                    }
                    break;
                case 2:
                    string keyCachedUpdate = $"{keyCached}_query_update";
                    StringBuilder builderUpdate = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase))
                            || c.FieldName.Equals(CommonKey.btsCreateDate)
                            || c.FieldName.Equals(CommonKey.btsCreateBy))
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        if (!entity.ContainsKey(fieldName))
                        {
                            continue;
                        }
                        ProcessDataBeforeSave(values, entity, c);
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        builderUpdate.Append($", `{fieldName}`= @{fieldName}");
                    }
                    query = $"UPDATEE `{tableName}` SET {builderUpdate.ToString().Substring(1)} WHERE `{primaryKeyName}` = @{primaryKeyName}";
                    TraceLogSaveEntity(query, values);
                    success = this.Execute(query, values);
                    break;
                case 3:
                    if (entity.ContainsKey(primaryKeyName))
                    {
                        query = $"DELETE FROM `{tableName}` WHERE `{primaryKeyName}` = @{primaryKeyName}";
                        values.AddOrUpdateDictionary(primaryKeyName, entity[primaryKeyName]);
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values);
                    }
                    break;
            }
            return success;
        }
        /// <summary>
        /// Lưu và trả về ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        public long SaveEntityReturnID(object entity, int entityState, string tableName, params string[] columsIgnore)
        {
            if (entity == null)
            {
                return 0;
            }
            string keyCached = $"MYSQLDatabaseInfo_{ _connectionString}_{tableName}".MD5Hash();
            List<Columns> columns = new List<Columns>();
            if (!disSchemas.TryGetValue(keyCached, out columns))
            {
                string queryCOLUMN_INFO = $@"SELECT COLUMN_NAME as FieldName,DATA_TYPE as DataType,
                                         CHARACTER_MAXIMUM_LENGTH as MaxLength, CASE WHEN COLUMN_KEY='PRI' THEN 1 ELSE 0 END AS IsIdentity,
                                         CASE WHEN IS_NULLABLE='YES' THEN 1 ELSE 0 END as IsNullable, Extra FROM Informatin_schema.Columns 
                                         WHERE Table_Name = @Table_name AND Table_Schema =database() ORDER BY Column_Name";
                columns = this.Query<Columns>(queryCOLUMN_INFO, new { Table_Name = tableName });
                if (columns != null && columns.Count > 0)
                {
                    if (!disSchemas.ContainsKey(keyCached))
                    {
                        lock (disSchemas)
                        {
                            disSchemas.Add(keyCached, columns);
                        }
                    }
                }
            }
            Columns columnsPK = columns.Find(m => m.IsIdentity);
            bool autoIncrenment = "auto_increment".Equals(columnsPK.Extra, StringComparison.OrdinalIgnoreCase);
            Dictionary<string, object> values = new Dictionary<string, object>();
            string query = string.Empty;
            string primaryKeyName = columnsPK.FieldName;
            long success = 0;
            switch (entityState)
            {
                case 1:
                    StringBuilder builderColumns = new StringBuilder();
                    StringBuilder builderValues = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        var pInfo = entity.GetType().GetProperty(fieldName);
                        if (pInfo == null)
                        {
                            continue;
                        }
                        builderColumns.Append($",`{fieldName}`");
                        builderValues.Append($",@{fieldName}");
                        ProcessDataBeforeSave(values, entity, c);
                    }
                    if (autoIncrenment)
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)}); SELECT LAST_INSERT_ID()";
                        TraceLogSaveEntity(query, values);
                        object primaryKeyValue = this.ExecuteScalar(query, values);
                        if (primaryKeyValue != null)
                        {
                            entity.SetValue(primaryKeyName, primaryKeyValue);
                            success = Convert.ToInt64(primaryKeyValue);
                        }

                    }
                    else
                    {
                        query = $"INSERT INTO `{tableName}` ({builderColumns.ToString().Substring(1)}) VALUES({builderValues.ToString().Substring(1)});";
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values);
                    }
                    break;
                case 2:
                    string keyCachedUpdate = $"{keyCached}_query_update";
                    StringBuilder builderUpdate = new StringBuilder();
                    foreach (var c in columns)
                    {
                        if (columsIgnore != null && columsIgnore.Any(m => m.Equals(c.FieldName, StringComparison.OrdinalIgnoreCase))
                            || c.FieldName.Equals(CommonKey.btsCreateDate)
                            || c.FieldName.Equals(CommonKey.btsCreateBy))
                        {
                            continue;
                        }
                        string fieldName = c.FieldName;
                        var pInfo = entity.GetType().GetProperty(fieldName);
                        if (pInfo ==null)
                        {
                            continue;
                        }
                        ProcessDataBeforeSave(values, entity, c);
                        if (c.IsIdentity && autoIncrenment)
                        {
                            continue;
                        }
                        builderUpdate.Append($", `{fieldName}`= @{fieldName}");
                    }
                    query = $"UPDATEE `{tableName}` SET {builderUpdate.ToString().Substring(1)} WHERE `{primaryKeyName}` = @{primaryKeyName}";
                    TraceLogSaveEntity(query, values);
                    success = this.Execute(query, values);
                    break;
                case 3:
                    var dInfo = entity.GetType().GetProperty(primaryKeyName);
                    if (dInfo != null)
                    {
                        query = $"DELETE FROM `{tableName}` WHERE `{primaryKeyName}` = @{primaryKeyName}";
                        values.AddOrUpdateDictionary(primaryKeyName, entity.GetValue<object>(primaryKeyName));
                        TraceLogSaveEntity(query, values);
                        success = this.Execute(query, values);
                    }
                    break;
            }
            return success;
        }
        /// <summary>
        ///  chuyển đổi từ Dic param sang  dicoutput param
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void MappingParamsDictionary(Dictionary<string, object> param ,MySqlCommand cmd, ref Dictionary<int, string> indexOutputParams)
        {
            if(param == null || param.Count ==0)
            {
                return;
            }   
            if(cmd.CommandType == CommandType.StoredProcedure)
            {
                MappingParamsDictionaryWithParamStore(param, cmd, ref indexOutputParams);
            }    
            else
            {
                foreach (KeyValuePair<string,object> pair in param)
                {
                    this.AddWithValue(cmd, pair.Key, pair.Value);
                }
            }
        }
        /// <summary>
        ///  chuển đổi Sang Param
        /// </summary>
        /// <param name="dicParam"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void MappingParamsDictionaryWithParamStore(Dictionary<string, object> dicParam, MySqlCommand cmd, ref Dictionary<int, string> indexOutputParams)
        {
            this.DB.DiscoverParameters(cmd);
            int totalParams = cmd.Parameters.Count;
            for (int i = 0; i < totalParams; i++)
            {
                DbParameter paRam = cmd.Parameters[i];
                string paramName = paRam.ParameterName.Replace("@", "");
                if(paRam.Direction == ParameterDirection.Output || paRam.Direction == ParameterDirection.InputOutput)
                {
                    indexOutputParams.Add(i, paramName);
                }
                if(dicParam.ContainsKey(paramName))
                {
                    object vVal = dicParam[paramName];
                    paRam.Value = vVal ?? DBNull.Value;
                }
                else
                {
                    paRam.Value = DBNull.Value;
                }
            }
        }
        /// <summary>
        ///  Mapping từ entity sang Dic param
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void MappingParamsDictionaryWithParamStore(object entity, MySqlCommand cmd, ref Dictionary<int, string> indexOutputParams)
        {
            this.DB.DiscoverParameters(cmd);
            int totalParams = cmd.Parameters.Count;
            Type objTye = entity.GetType();
            for (int i = 0; i < totalParams; i++)
            {
                DbParameter paRam = cmd.Parameters[i];
                string paramName = paRam.ParameterName.Replace("@", "");
                if (paRam.Direction == ParameterDirection.Output || paRam.Direction == ParameterDirection.InputOutput)
                {
                    indexOutputParams.Add(i, paramName);
                }
                PropertyInfo pi = objTye.GetProperty(paramName);
                if(pi != null)
                {
                    object vVall = pi.GetValue(entity);
                    if(vVall != null)
                    {
                        paRam.Value = vVall;
                    }
                    else
                    {
                        paRam.Value = DBNull.Value;
                    }
                }
                else
                {
                    paRam.Value = DBNull.Value;
                }
            }
        }
        /// <summary>
        ///  chuyển đồi từ object param sang Dic param
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void MappingParamsObject(object param, MySqlCommand cmd, ref Dictionary<int, string> indexOutputParams)
        {
            if(param == null)
            {
                return;
            }
            if(cmd.CommandType == CommandType.StoredProcedure)
            {
                MappingParamsDictionaryWithParamStore(param, cmd,ref indexOutputParams);
            }
            else
            {
                PropertyInfo[] fs = param.GetType().GetProperties();
                foreach (PropertyInfo f in fs)
                {
                    string opName = f.Name;
                    object vVal = f.GetValue(param);
                    this.AddWithValue(cmd, opName, vVal);
                }
            }
        }
        /// <summary>
        /// Set giá trị Param  từ object
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void SetOutPutParamsObject(object param, MySqlCommand cmd, Dictionary<int,string> indexOutputParams)
        {
            if(param == null || indexOutputParams.Count ==0)
            {
                return;
            }
            foreach (KeyValuePair<int, string> pair in indexOutputParams)
            {
                var outputData = cmd.Parameters[pair.Key].Value;
                param.SetValue(pair.Value, outputData);
            }
        }
        /// <summary>
        /// Set giá trị Param  từ Dic
        /// </summary>
        /// <param name="dicParam"></param>
        /// <param name="cmd"></param>
        /// <param name="indexOutputParams"></param>
        private void SetOutPutParamsDictionary(Dictionary<string, object> dicParam, MySqlCommand cmd, Dictionary<int, string> indexOutputParams)
        {

            if (dicParam == null || dicParam.Count == 0)
            {
                return;
            }
            foreach(KeyValuePair<int, string> pair in indexOutputParams)
            {
                var outputData = cmd.Parameters[pair.Key].Value;
                dicParam[pair.Value] = outputData;
            }
        }
        /// <summary>
        /// cleadr Sql Data
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="dbCommand"></param>
        private void ClearDataAccess(IDataReader rd , IDbCommand dbCommand)
        {

            try
            {
                if(rd != null)
                {
                    rd.Close();
                    rd.Dispose();
                }
                if(dbCommand != null)
                {
                    dbCommand.Dispose();
                }
                if(transaction == null)
                {
                    CloseConn();
                }

            }
            catch(Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Lấy ra giá trị dầu tiên của bảng object param
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public T QueryFistOrDefault<T>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            T rs = default(T);
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListObject<T>().FirstOrDefault();
            }, param, commandType, timeOut);
            return rs;
        }
        /// <summary>
        /// lấy giá trị đầu tiên của table theo Dic param
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public T QueryFistOrDefault<T>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            T rs = default(T);
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListObject<T>().FirstOrDefault();
            }, param, commandType, timeOut);
            return rs;
        }
        /// <summary>
        /// Lấy ra danh sách
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public List<T> Query<T>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            List<T> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListObject<T>();
            }, param, commandType, timeOut);
            return rs;
        }
        /// <summary>
        /// Lấy ra ds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>

        public List<T> Query<T>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            List<T> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListObject<T>();
            }, param, commandType, timeOut);
            return rs;
        }
        /// <summary>
        /// Lấy ra ds
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>

        public IList Query(string query, Type type, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            List<Dictionary<string, object>> rs = null;
            this.ProcessIDataRender(query, (rd) =>
            {
                rd.ToListObject(type);
            }, param, commandType, timeOut);
            return rs;
        }
        /// <summary>
        /// Lấy ra lyst
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>

        public List<Dictionary<string, object>> QueryListDictionary(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            List<Dictionary<string, object>> rs = new List<Dictionary<string, object>>();
            this.ProcessIDataRender(query, (rd) =>
            {
                rs = rd.ToListDictionary();
            }, param, commandType, timeOut);
            return rs;
        }

        public List<T> QueryCustom<T>(string query, object param = null)
        {
            MySqlDataReader rd = null;
            MySqlCommand cmd = null;
            MySqlConnection connectionNyConnectionName = null;
            try
            {
                var connectionString = "";
                connectionNyConnectionName = new MySqlConnection(connectionString);
                connectionNyConnectionName.Open();
                cmd = new MySqlCommand(query, connectionNyConnectionName);
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsObject(param, cmd, ref indexOutputParams);
                rd = cmd.ExecuteReader();
                return rd.ToListObject<T>();
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(rd, cmd);
                if(connectionNyConnectionName !=null)
                {
                    connectionNyConnectionName.Dispose();
                    connectionNyConnectionName = null;
                }
            }
        }

        public void ProcessIDataRender(string query, Action<IDataReader> action, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlDataReader rd = null;
            MySqlCommand cmd = null;
            try
            {
                cmd = CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsDictionary(param, cmd, ref indexOutputParams);
                if (transaction != null)
                {
                    rd = DB.ExcuteRender(cmd, transaction);
                }
                else
                {
                    rd = DB.ExcuteRender(cmd);
                }
                SetOutPutParamsDictionary(param, cmd, indexOutputParams);
                action(rd);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(rd, cmd);
            }
        }

        public void ProcessIDataRender(string query, Action<IDataReader> action, object param, CommandType? commandType = CommandType.Text, int? timeOut = 150)
        {
            MySqlDataReader rd = null;
            MySqlCommand cmd = null;
            try
            {
                cmd = CreateCommand();
                cmd.CommandType = commandType.Value;
                cmd.CommandText = query;
                cmd.CommandTimeout = timeOut.Value;
                Dictionary<int, string> indexOutputParams = new Dictionary<int, string>();
                MappingParamsObject(param, cmd, ref indexOutputParams);
                if(transaction != null)
                {
                    rd = DB.ExcuteRender(cmd, transaction);
                }
                else
                {
                    rd = DB.ExcuteRender(cmd);
                }
                SetOutPutParamsObject(param, cmd, indexOutputParams);
                action(rd);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
               ClearDataAccess(rd, cmd);
            }
        }

        public int BulkInsertMySQL(DataTable table, string tableName, string colums = "*", int? timeOut = 150)
        {
            MySqlDataReader rd = null;
            MySqlCommand cmd = null;
            int row = -1;
            try
            {
                cmd = CreateCommand();
                if(string.IsNullOrEmpty(colums))
                {
                    colums = "*";
                }
                cmd.CommandText = $"SELECT {colums} FROM " + tableName + "`limit 0`";
                cmd.CommandTimeout = timeOut.Value;
                if( transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                using(MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.UpdateBatchSize = 10000;
                    using (MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter))
                    {
                        cb.SetAllValues = true;
                        row = adapter.Update(table);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(rd, cmd);
            }
            return row;
        }

        public bool BulkUpdateMySQL(DataTable table, string tableName, string primaryKeyName, string colums = "*", int? timeOut = 150)
        {
            MySqlDataReader rd = null;
            MySqlCommand cmd = null;
            try
            {
                if(SecureUtil.DetectSqlInjection(primaryKeyName))
                {
                    throw new Exception($"{primaryKeyName} was SqlInjection");
                }
                if (SecureUtil.DetectSqlInjection(colums))
                {
                    throw new Exception($"{colums} was SqlInjection");
                }
                if (SecureUtil.DetectSqlInjection(tableName))
                {
                    throw new Exception($"{tableName} was SqlInjection");
                }
                DataColumnCollection colsCollection = table.Columns;
                HashSet<string> setFeilds = new HashSet<string>();
                HashSet<string> ids = new HashSet<string>();
                foreach (DataColumn item in colsCollection)
                {
                    if(primaryKeyName.Equals(item.ColumnName,StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if(!"*".Equals(colums) && colums.IndexOf(item.ColumnName) == -1)
                    {
                        continue;
                    }
                    string caseWhen = $"{item.ColumnName} =(CASE `{primaryKeyName}1`";
                    foreach (DataRow dataRow in table.Rows)
                    {
                        object primaryKeyValue = dataRow[primaryKeyName];
                        if(primaryKeyValue.GetType() == typeof(string))
                        {
                            if(SecureUtil.DetectSqlInjection(primaryKeyValue.ToString()))
                            {
                                throw new Exception($"{primaryKeyValue} was SqlInjection");
                            }
                            if(primaryKeyValue.GetType() == typeof(DateTime))
                            {
                                ids.Add($"'{Convert.ToDateTime(primaryKeyValue).ToString("yyyy/MM/dd HH:mm:ss")}'");
                            }
                            else
                            {
                                ids.Add($"'{primaryKeyValue.ToString()}");
                            }   
                        }
                        else
                        {
                            ids.Add($"'{primaryKeyValue.ToString()}");
                        }
                        object cellValue = dataRow[item.ColumnName];
                        caseWhen += $" WHEN {primaryKeyValue} THEN {ConvertValueByDataType(item, cellValue)}";
                    }
                    caseWhen += " END)";
                    setFeilds.Add(caseWhen);
                }
                string queryUpdate = $"'UPDATE `{tableName}` SET { string.Join(",", setFeilds)} WHERE `{primaryKeyName}` IN({string.Join(",", ids)})'";
                return this.Execute(queryUpdate) > 0;
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDataAccess(rd, cmd);
            }
        }
        private string ConvertValueByDataType(DataColumn column, object value)
        {
            if(value == null && DBNull.Value.Equals(value))
            {
                return "null";
            }    
            switch(column.DataType.Name)
            {
                case "Boolean":
                    return $"{Convert.ToBoolean(value)}";
                case "Byte":
                    return $"{value}";
                case "Char":
                    return $"'{value.ToString().Replace("'", "''")}'";
                case "DateTime":
                    return $"{Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss")}";
                case "Decimal":
                    return $"{value}";
                case "Double":
                    return $"{value}";
                case "Int16":
                    return $"{value}";
                case "Int32":
                    return $"{value}";
                case "Int64":
                    return $"{value}";
                case "SByte":
                    return $"{value}";
                case "Single":
                    return $"{value}";
                case "String":
                    return $"{SecureUtil.SafeSqlLiteral(value.ToString().Replace("'", "''"))}";
                case "TimeSpan":
                    return $"{value}";
                case "UInt16":
                    return $"{value}";
                case "UInt32":
                    return $"{value}";
                case "UInt64":
                    return $"{value}";
            }
            return string.Empty;
        }
    }
}
