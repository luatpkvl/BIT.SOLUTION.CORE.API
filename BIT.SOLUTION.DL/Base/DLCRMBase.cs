using BIT.SOLUTION.Common;
using BIT.SOLUTION.DbHepper;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.Lib.Enum;
using BIT.SOLUTION.Lib.SessionData;
using BIT.SOLUTION.MODEL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BIT.SOLUTION.DL
{
    public partial class DLCRMBase : DLBase,IDLCRMBase
    {
        public DLCRMBase(IUnitOfWork unitOfWork):base(unitOfWork)
        {

        }
        public virtual IEnumerable<T> GetAll<T>()
        {
            string query = $"SELECT * From {Utility.GetTableName<T>()}";
            return this.UnitOfWork.Query<T>(query);

        }
        public virtual bool DeleteDataByID(object deleteID, string tableName)
        {
            string query = $"DELETE FROM {tableName} WHERE ID+@ID";
            return this.UnitOfWork.Execute(query, new { ID = deleteID }) > 0;
        }
        public virtual bool DeleteData<T>(object id) 
        {
            string query = $"DELETE  From {Utility.GetTableName<T>()} WHERE ID = @ID";
            return this.UnitOfWork.Execute(query, new { ID = id }) > 0;
        }

        public virtual bool DeleteData(object id, string tableName = "")
        {
            string query = $"DELETE From {tableName} WHERE ID = @ID";
            return this.UnitOfWork.Execute(query,new { ID =id })> 0;
        }

        public virtual T GetDataByID<T>(object id, string column = "*")
        {
            string query = $"SELECT {column} From {Utility.GetTableName<T>()} WHERE ID = @ID";
            return this.UnitOfWork.QueryFistOrDefault<T>(query, new { ID =id });
        }
        /// <summary>
        /// Lay Data theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetDataByID(object id, string tableName, string column = "*")
        {
            tableName = SecureUtil.SafeSqlLiteral(tableName);
            string query = $"SELECT {column} FROM {tableName} WHERE ID =@ID";
            var dicData = this.UnitOfWork.QueryListDictionary(query, new { ID = id }).FirstOrDefault();
            return dicData;

        }
        public virtual List<Dictionary<string, object>> GetListData(string tableName, string column = "*")
        {
            tableName = SecureUtil.SafeSqlLiteral(tableName);
            string query = $"SELECT {column} FROM {tableName}";
            var dicData = this.UnitOfWork.QueryListDictionary(query);
            return dicData;
        }
        /// <summary>
        /// Lay theo ds ID
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="enities"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual List<Dictionary<string, object>> GetDataByIDS(string tableName = "", List<long> enities = null, string column = "*")
        {
            string query = $"SELECT {column} From {tableName} WHERE ID in = {String.Join(",", enities)}";
            return this.UnitOfWork.QueryListDictionary(query);

        }
        public virtual bool Save(BaseEntity entity)
        {
            bool success = false;
            try
            {
                if(entity.UserProc)
                {
                    success = this.SaveByProc(entity);
                }
                else
                {
                    success = this.SaveByCmdText(entity);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return success;
        }
        private string GetQueryCmdText(BaseEntity entity)
        {
            string query = string.Empty;
            switch(entity.BITEntityState)
            {
                case Enumeration.BITEntityState.Add:
                    query = entity.QueryInsert();
                    break;
                case Enumeration.BITEntityState.Edit:
                    query = entity.QueryInsert();
                    break;
                case Enumeration.BITEntityState.Delete:
                    query = entity.QueryInsert();
                    break;
            }
            return query;
        }
        private bool SaveByCmdText(BaseEntity objEntity)
        {
            bool success = false;
            int iRetries = 4;
            while(iRetries >0)
            {
                try
                {
                    string query = this.GetQueryCmdText(objEntity);
                    string primaryKeyName = objEntity.GetPrimaryKeyFieldName();
                    Dictionary<string, object> dicParameters = new Dictionary<string, object>();
                    void AddDicParameters(bool isIdentity, List<string> columns, bool isJson  = false)
                    {
                        foreach (var c in columns)
                        {
                            if(isIdentity && objEntity.BITEntityState == Enumeration.BITEntityState.Add
                                && c.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            PropertyInfo pInfo = objEntity.GetEntityType().GetProperty(c);
                            object vVal = pInfo.GetValue(objEntity);
                            if(vVal  != null && isJson)
                            {
                                dicParameters.Add(c, CommonFnConvert.SerializableObject(vVal));
                            }
                            else
                            {
                                dicParameters.Add(c, vVal);
                            }
                        }
                    }
                    switch(objEntity.BITEntityState)
                    {
                        case Enumeration.BITEntityState.Add:
                        case Enumeration.BITEntityState.Edit:
                            string tableName = objEntity.GetTableName();
                            bool isIdentity = !objEntity.KeyIsGuid();
                            var tupleColumn = objEntity.GetColumns();
                            AddDicParameters(isIdentity, tupleColumn.Item1, true);
                            AddDicParameters(isIdentity, tupleColumn.Item2, true);
                            void AddExtraParam(string propertyName, object value)
                            {
                                if(objEntity.ExitstProperty(propertyName))
                                {
                                    if(!dicParameters.ContainsKey(propertyName))
                                    {
                                        dicParameters.Add(propertyName, value);
                                    }
                                    else
                                    {
                                        dicParameters[propertyName] = value;
                                    }
                                }
                            }
                            objEntity.SetValue(CommonKey.btsModifiedBy, !string.IsNullOrEmpty(objEntity.ModifiedBy) ? objEntity.ModifiedBy : SessionData.FullName);
                            objEntity.SetValue(CommonKey.btsModifiedDate, DateTime.Now);
                            AddExtraParam(CommonKey.btsModifiedBy, !string.IsNullOrEmpty(objEntity.ModifiedBy) ? objEntity.ModifiedBy : SessionData.FullName);

                            AddExtraParam(CommonKey.btsModifiedDate, DateTime.Now);
                            if(objEntity.BITEntityState == Enumeration.BITEntityState.Add)
                            {
                                objEntity.SetValue(CommonKey.btsModifiedBy, !string.IsNullOrEmpty(objEntity.CreateBy) ? objEntity.CreateBy : SessionData.FullName);
                                objEntity.SetValue(CommonKey.btsModifiedDate, DateTime.Now);

                                AddExtraParam(CommonKey.btsModifiedBy, !string.IsNullOrEmpty(objEntity.CreateBy) ? objEntity.CreateBy : SessionData.FullName);

                                AddExtraParam(CommonKey.btsModifiedDate, DateTime.Now);
                                if(objEntity.ExitstProperty(CommonKey.bitAsyncID))
                                {
                                    Guid asyncId = objEntity.GetValue<Guid>(CommonKey.bitAsyncID);
                                    if(asyncId ==null || string.Compare(Guid.Empty.ToString(),asyncId.ToString(),true) ==0)
                                    {
                                        asyncId = Guid.NewGuid();
                                        objEntity.SetValue(CommonKey.bitAsyncID, asyncId);
                                    }
                                    AddExtraParam(CommonKey.bitAsyncID, asyncId);
                                }
                                if(isIdentity)
                                {
                                    object primaryKeyValue = this.UnitOfWork.ExecuteScalar(query, dicParameters);
                                    if(primaryKeyValue != null)
                                    {
                                        success = true;
                                        objEntity.SetValue(primaryKeyName, primaryKeyValue);
                                    }
                                }
                                else
                                {
                                    success = this.UnitOfWork.Execute(query, dicParameters) > 0;
                                }
                            }
                            else
                            {
                                success = this.UnitOfWork.Execute(query, dicParameters) > 0;
                            }
                            break;
                        case Enumeration.BITEntityState.Delete:
                            success = this.UnitOfWork.Execute(query, new Dictionary<string, object> { { primaryKeyName, objEntity.GetObjectPrimaryKeyValue() } }) > 0;
                            break;
                        case Enumeration.BITEntityState.None:
                            success = true;
                            break;
                    }
                    iRetries = 0;
                }
                catch(MySqlException e)
                {
                    if( e.Number ==1205 || e.Number ==1213)
                    {
                        iRetries -= 1;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        throw e;
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            return success;
        }
        private bool SaveByProc(BaseEntity entity)
        {
            bool success = false;
            string storeName = this.GetStoreNambyEditMode(entity);
            if(!string.IsNullOrEmpty(storeName))
            {
                int iRetries = 4;
                while(iRetries >0)
                {
                    try
                    {
                        success = this.UnitOfWork.Execute(storeName, entity,commandType: CommandType.StoredProcedure) >0;
                    }
                    catch(MySqlException e)
                    {
                        if(e.Number ==1205 || e.Number ==1213)
                        {
                            iRetries -= 1;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }
            }
            else
            {
                /// Ghi logger
                throw new Exception("Strore not foud");
            }
            return success;
        }
        private string GetStoreNambyEditMode(BaseEntity entity)
        {
            string storeName = string.Empty;
            if(entity != null)
            {
                switch(entity.BITEntityState)
                {
                    case Enumeration.BITEntityState.Add:
                        storeName = string.Format(bitProctInsert, entity.GetType().Name);
                        break;
                    case Enumeration.BITEntityState.Edit:
                        storeName = string.Format(bitProctUpdate, entity.GetType().Name);
                        break;
                    case Enumeration.BITEntityState.UpdateInsert:
                        storeName = string.Format(bitProctUpdate, entity.GetType().Name);
                        break;
                    default:
                        storeName = string.Empty;
                        break;
                }
            }
            return storeName;
        }
        public bool IsModule(string tableName)
        {
            return Enum.GetNames(typeof(Enumeration.LayoutCode)).Any(m => m.Equals(tableName, StringComparison.OrdinalIgnoreCase));
        }
        public bool DeleteByCmdText(BaseEntity entity, string tableName = "", string primaryKeyName = "")
        {
            bool isSucess = false;
            int iRetries = 4;
            while(iRetries >0)
            {
                try
                {
                    if (primaryKeyName == "")
                    {
                        primaryKeyName = entity.GetPrimaryKeyFieldName();
                    }
                    if (tableName == "")
                    {
                        tableName = entity.GetTableName();
                    }
                    string query = $"DELETE FROM `{tableName}` WHERE {primaryKeyName} =@{primaryKeyName}";
                    bool isModule = this.IsModule(entity.GetType().Name);
                    if (isModule && entity.IsDeletePermanently != true)
                    {
                        query = $"UPDATE `{tableName}` SET IsDeleted =1, ModifiedDate = now(), ModifiedBy =@ModifiedBy WHERE {primaryKeyName} = @{primaryKeyName}";
                    }
                    Dictionary<string, object> dicParameters = new Dictionary<string, object>();
                    object vVal = entity.GetPrimaryKeyValue();
                    dicParameters.Add(primaryKeyName, vVal);
                    dicParameters.Add("@ModifiedBy", SessionData.FullName ?? string.Empty);
                    isSucess = this.UnitOfWork.Execute(query, dicParameters) > 0;
                    iRetries = 0;
                }
                catch (MySqlException e)
                {
                    if (e.Number == 1205 || e.Number == 1213)
                    {
                        iRetries -= 1;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        throw e;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
               
            }
            return isSucess;
        }
    }
}
