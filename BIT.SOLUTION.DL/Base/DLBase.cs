using BIT.SOLUTION.Data;
using BIT.SOLUTION.DbHepper;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BIT.SOLUTION.DL
{
    public partial class DLBase : IDLBase
    {
        #region Delaration
        private IUnitOfWork _unitOfWork;
        private static object lockTaskUnitOfWork = new object();
        public virtual Dictionary<string,IUnitOfWork> TaskUnitOfWork { set; get; }
        public virtual IUnitOfWork UnitOfWork
        {
            get
            {
                string threadName = Thread.CurrentThread.ManagedThreadId.ToString();
                if(this.TaskUnitOfWork == null)
                {
                    this.TaskUnitOfWork = new Dictionary<string, IUnitOfWork>();
                }
                IUnitOfWork unitOfWork = null;
                if(!this.TaskUnitOfWork.TryGetValue(threadName, out unitOfWork))
                {
                    lock(lockTaskUnitOfWork)
                    {
                        unitOfWork = new UnitOfWork();
                        this.TaskUnitOfWork.Add(threadName, unitOfWork);
                    }
                }
                return unitOfWork;
            }
            set
            {
                string threadName = Thread.CurrentThread.ManagedThreadId.ToString();
                if (this.TaskUnitOfWork == null)
                {
                    this.TaskUnitOfWork = new Dictionary<string, IUnitOfWork>();
                }
                lock (lockTaskUnitOfWork)
                {
                    if(this.TaskUnitOfWork.ContainsKey(threadName))
                    {
                        this.TaskUnitOfWork.Add(threadName, value);
                    }
                    else
                    {
                        this.TaskUnitOfWork[threadName] = value;
                    }
                }
                this._unitOfWork = value;
            }
        }
        public virtual SqlDataBase DB
        {
            get
            {
                return this.UnitOfWork.DB;
            }
        }
        #endregion
        #region CTOR
        public DLBase(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        #endregion
        #region FUC
        public void BeginTransaction()
        {
            this.UnitOfWork.BeginTransaction();
        }

        public void Commit()
        {
            this.UnitOfWork.Commit();
        }
       
        public void CreateTaskUnitWork(string companyCode)
        {
            if(!string.IsNullOrEmpty(Thread.CurrentThread.ManagedThreadId.ToString()))
            {
                if(this.TaskUnitOfWork != null)
                {
                    if(!this.TaskUnitOfWork.ContainsKey(Thread.CurrentThread.ManagedThreadId.ToString()))
                    {
                        lock(this.TaskUnitOfWork)
                        {
                            this.TaskUnitOfWork.Add(Thread.CurrentThread.ManagedThreadId.ToString(), new UnitOfWork());
                        }
                    }
                }
                else
                {
                    this.TaskUnitOfWork = new Dictionary<string, IUnitOfWork>();
                    lock(this.TaskUnitOfWork)
                    {
                        this.TaskUnitOfWork.Add(Thread.CurrentThread.ManagedThreadId.ToString(), new UnitOfWork());
                    }
                }
                UnitOfWork.CreateSQL(companyCode);
            }
        }

        public void DestroyTaskUnitWork(string companyCode)
        {
            string threadName = Thread.CurrentThread.ManagedThreadId.ToString();
            if(!string.IsNullOrEmpty(threadName))
            {
                IUnitOfWork unit = null;
                if(this.TaskUnitOfWork.TryGetValue(threadName, out unit))
                {
                    unit.Dispose();
                    lock(this.TaskUnitOfWork)
                    {
                        this.TaskUnitOfWork.Remove(threadName);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.UnitOfWork.Dispose();
        }
        /// <summary>
        /// lay data ra data set
        /// </summary>
        /// <param name="storeProc"></param>
        /// <param name="paramsVal"></param>
        /// <returns></returns>

        public DataSet ExecuteDataSet(string storeProc, params string[] paramsVal)
        {
            DataSet ds = new DataSet();
            try
            {
                using(SqlCommand cmd = this.UnitOfWork.GetStoreProcCommand(storeProc,paramsVal))
                {
                    ds = DB.ExcuteDataSet(cmd);
                }
            }
            catch(Exception e)
            {
                throw;
            }
            return ds;
        }

        public bool ExecuteNoneQuery(string storeProc, params string[] paramsVal)
        {
            try
            {
                bool succes = false;
                var cmd = UnitOfWork.GetStoreProcCommand(storeProc, paramsVal);
                succes = this.UnitOfWork.ExecuteNoneQueryCmd(cmd);
                return succes;
            }
            catch(MySqlException se)
            {

                throw se;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public object ExecuteScalar(string storeProc, params string[] paramsVal)
        {
            try
            {
                var cmd = UnitOfWork.GetStoreProcCommand(storeProc, paramsVal);
                return this.UnitOfWork.ExecuteScalarCmd(cmd);
            }
            catch (MySqlException se)
            {

                throw se;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual T GetDataCmdTextByID<T> (object id, string primaryKey, string tableName)
        {
            primaryKey = SecureUtil.SafeSqlLiteral(primaryKey);
            string query = $"SELECT * FROM `{SecureUtil.SafeSqlLiteral(tableName)}` WHERE `{primaryKey}` =@{primaryKey}";
            return this.UnitOfWork.QueryFistOrDefault<T>(query, new Dictionary<string, object> { { primaryKey, id } });
        }
        public object GetDataCmdTextByID(BaseEntity entity)
        {
            string primaryKey = entity.GetPrimaryKeyFieldName();
            object ID = entity.GetPrimaryKeyValue();
            string query = $"SELECT * FROM `{entity.GetTableName()}` WHERE `{primaryKey}` =@{primaryKey}";
            IList lstResult = this.UnitOfWork.Query(query, entity.GetType(), new { ID = ID });
            if(lstResult != null && lstResult.Count >0)
            {
                return lstResult;
            }
            return null;
        }

        public IDataReader GetDataReader(string storeProc, params string[] paramsVal)
        {
            IDataReader bjDataReader = null;
            try
            {
                var cmd = UnitOfWork.GetStoreProcCommand(storeProc, paramsVal);
                bjDataReader = this.UnitOfWork.ExecuteRenderCmd(cmd);
            }
            catch(Exception e)
            {
                throw e;
            }
            return bjDataReader;
        }

        public Dictionary<string, object> GetPermissionSetting()
        {
            throw new NotImplementedException();
        }

        public void RollBack()
        {
            this.UnitOfWork.RollBack();
        }

        protected string GetConnectionString()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
