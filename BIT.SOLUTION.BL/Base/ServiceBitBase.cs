using BIT.SOLUTION.Common;
using BIT.SOLUTION.DL;
using BIT.SOLUTION.Lib;
using BIT.SOLUTION.Lib.Enum;
using BIT.SOLUTION.Lib.SessionData;
using BIT.SOLUTION.MODEL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.SERVICE
{
    public partial class ServiceBitBase : ServiceBase, IServiceBitBase
    {
        private readonly IDLCRMBase DL;
        public ServiceBitBase(IDLCRMBase dl) : base(dl)
        {
            DL = dl;
        }
        public virtual IEnumerable<T> GetDataAll<T>()
        {
           return this.DL.GetAll<T>();
        }
        public virtual List<Dictionary<string, object>> GetDataAll<T>(string column = "*")
        {
            string tableName = Utility.GetTableName<T>();
            return this.DL.GetListData(tableName);
        }
        /// <summary>
        /// Truoc khi xoa
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="serviceResult"></param>
        protected virtual void BeforeDelete(BaseEntity entity, ServiceResult serviceResult)
        {
            return;
        }
        /// <summary>
        /// Xoas duwx lieu
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        protected virtual void DoDelete(BaseEntity entity, string tableName, string primaryKeyName)
        {
            this.DL.DeleteByCmdText(entity, tableName, primaryKeyName);
            if (entity.DeleteTableRelation != null)
            {
                foreach (var tableRelation in entity.DeleteTableRelation)
                {
                    this.DL.DeleteDataByID(entity.GetPrimaryKeyValue(), SecureUtil.SafeSqlLiteral(tableRelation));
                }
            }
        }
        /// <summary>
        /// XOa theo entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <param name="prmaryKeyName"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual ServiceResult Delete<T>(BaseEntity entity, string tableName = "", string prmaryKeyName = "", List<T> entities = null)
        {
            ServiceResult oResult = new ServiceResult();
            try
            {
                // xu ly truoc khi xoa
                this.BeforeDelete(entity, oResult);
                /// validate truoc khi xoa
                List<ValidateResult> lstValidateResults = new List<ValidateResult>();
                if (lstValidateResults.Count == 0)
                {
                    this.DL.BeginTransaction();
                    // xoa
                    this.DoDelete(entity, tableName, prmaryKeyName);
                    /// sau khi xoa
                    this.AfterDelete(entity, oResult);
                    if (oResult.Success)
                    {
                        this.DL.Commit();
                        this.AfterDeleteCommit(entity);
                        Task.Run(() =>
                        {
                            this.AsyncAfterDelete(entity, oResult);
                        });
                    }
                }
            }
            catch (SqlException e)
            {
                this.DL.RollBack();
                oResult.SetError(e);
            }
            catch (Exception e)
            {
                this.DL.RollBack();
                oResult.SetError(e);
            }
            finally
            {
                this.DL.Dispose();
            }
            return oResult;
        }
        /// <summary>
        /// Thực hiện ẩn với luồng riêng không ảnh hưởng luofng chính
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="oResult"></param>
        protected virtual void AsyncAfterDelete(BaseEntity entity, ServiceResult oResult)
        {
            return;
        }
        /// <summary>
        /// Sau khi commit
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AfterDeleteCommit(BaseEntity entity)
        {
            return;
        }
        /// <summary>
        /// Sau khi xóa
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="oResult"></param>
        protected virtual void AfterDelete(BaseEntity entity, ServiceResult oResult)
        {
            return;
        }
        /// <summary>
        ///  lay toi tuong theo id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetDataByID(object id, string tableName, string column = "*")
        {
            return this.DL.GetDataByID(id, tableName, column);
        }
        public virtual T GetDataByID<T>(object id, string column = "*")
        {
            return this.DL.GetDataByID<T>(id);
        }
        /// <summary>
        /// Trước khi Lưu
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sResult"></param>
        protected virtual void BeforeSave(BaseEntity entity, ServiceResult sResult)
        {
            if (entity.BITEntityState == Enumeration.BITEntityState.Add)
            {
                if (entity.Fields != null && entity.Fields.Count > 0)
                {
                    if (entity.ExitProperty(Constranst.OwnerID))
                    {
                        var ownerField = entity.Fields.Find(f => Constranst.OwnerID.Equals(f.FieldName));
                        if (ownerField == null)
                        {
                            entity.SetValue(Constranst.OwnerID, SessionData.BITUserID);
                            entity.SetValue(Constranst.OwnerIDText, SessionData.FullName);
                            entity.Fields.Add(new BITField
                            {
                                FieldName = Constranst.OwnerID,
                                Value = SessionData.BITUserID,

                            });
                            entity.Fields.Add(new BITField
                            {
                                FieldName = Constranst.OwnerIDText,
                                Value = SessionData.FullName,

                            });
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Lưu dữ liệu
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="detailFiedConfig"></param>
        /// <param name="lstLogs"></param>
        /// <returns></returns>
        protected virtual bool DoSave(BaseEntity entity, string[] detailFiedConfig, ref List<object> lstLogs)
        {
            bool success = true;
            if (entity.IsAuditLog && entity.BITEntityState == Enumeration.BITEntityState.Edit && entity.OldData == null)
            {
                entity.OldData = this.DL.GetDataCmdTextByID(entity);
            }
            success = this.DL.Save(entity);
            return success;
        }
        /// <summary>
        /// Sau khi lưu
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sResult"></param>
        protected virtual void AfterSave(BaseEntity entity, ServiceResult sResult)
        {
            return;
        }
        /// <summary>
        /// Sau khi commit
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AfterCommit(BaseEntity entity)
        {
            return;
        }
        /// <summary>
        /// Sau commit vơi luồng riêng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="sResult"></param>
        protected virtual void AsyncAfterSave<T>(BaseEntity entity, ServiceResult sResult)
        {
            return;
        }
        /// <summary>
        /// Lưu data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ServiceResult SaveData<T>(BaseEntity entity)
        {
            ServiceResult oResult = new ServiceResult();
            List<object> lstLog = new List<object>();
            try
            {
                /// truoc khi luu
                this.BeforeSave(entity, oResult);
                this.DL.UnitOfWork.BeginTransaction();
                var lstValidateResult = this.ValidateSaveData(entity);
                if (lstValidateResult.Count == 0)
                {
                    entity.BuildQuery();
                    this.DL.UnitOfWork.BeginTransaction();
                    oResult.Success = this.DoSave(entity, entity.DetailFoeldConfig, ref lstLog);
                    if (oResult.Success)
                    {
                        /// Xu ly sau khi luu
                        this.AfterSave(entity, oResult);
                    }
                    if (oResult.Success)
                    {
                        this.DL.UnitOfWork.Commit();
                        this.AfterCommit(entity);
                        if (entity != null && entity.GetObjectPrimaryKeyValue() != null)
                        {
                            long.TryParse(entity.GetObjectPrimaryKeyValue().ToString(), out long id);
                            Task.Run(() =>
                            {
                                this.AsyncAfterSave<T>(entity, oResult);
                            });
                        }
                    }
                    else
                    {
                        this.DL.UnitOfWork.RollBack();
                    }
                }
                else
                {
                    oResult.Success = false;
                    oResult.Code = System.Net.HttpStatusCode.InternalServerError;
                    oResult.ValidateInfo = lstValidateResult;
                }

            }
            catch (Exception e)
            {
                oResult.SetError(e);
            }
            finally
            {
                this.DL.UnitOfWork.Dispose();
            }
            return oResult;
        }
        /// <summary>
        ///  Save List data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void PreviewSaveListData<T>(List<T> entities) where T :BaseEntity
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public virtual void PreviewWithTranSaveListData<T>(List<T> entities) where T : BaseEntity
        {
        }
        public virtual void AfterCommitSaveListData<T>(List<T> entities) where T : BaseEntity
        {
        }
        public virtual List<ServiceResult> SaveListData<T>(List<T> entities) where T :BaseEntity
        {
            List<ServiceResult> result = new List<ServiceResult>();
            ServiceResult oResult = new ServiceResult() { Success = true };
            List<object> lstLog = new List<object>();
            try
            {
                if(entities == null || entities.Count ==0)
                {
                    return result;
                }
                PreviewSaveListData(entities);
                foreach (var entity in entities)
                {
                    oResult = new ServiceResult() { Success = true };
                    this.BeforeSave(entity, oResult);
                    List<ValidateResult> lstValidateResult = this.ValidateSaveData(entity);
                    if (lstValidateResult.Count > 0)
                    {
                        oResult.Success = false;
                        oResult.Code = System.Net.HttpStatusCode.InternalServerError;
                        oResult.ValidateInfo = lstValidateResult;
                        result.Add(oResult);
                        return result;
                    }
                    entity.BuildQuery();
                    if( entity.FieldsCustom !=null && entity.FieldsCustom.Count >0 )
                    {
                        //string tableColumn = entity.GetT
                    }
                }
                oResult = new ServiceResult();
                this.DL.UnitOfWork.BeginTransaction();
                PreviewWithTranSaveListData(entities);
                foreach (var entity in entities)
                {
                    oResult = new ServiceResult() { Success = true };
                    oResult.Success = this.DoSave(entity, entity.DetailFoeldConfig, ref lstLog);
                    if(!oResult.Success)
                    {
                        result.Add(oResult);
                        break;
                    }
                    this.AfterSave(entity, oResult);
                    if(!oResult.Success)
                    {
                        result.Add(oResult);
                        break;
                    }
                }
                if(!oResult.Success)
                {
                    this.DL.RollBack();
                    return result;
                }
                this.DL.UnitOfWork.Commit();
                foreach (var entity in entities)
                {
                    this.AfterCommit(entity);
                }
                if(oResult.Success)
                {
                    this.AfterCommitSaveListData<T>(entities);
                    List<long> lstIDs = new List<long>();
                    foreach (var item in entities)
                    {
                        lstIDs.Add(long.Parse(item.GetPrimaryKeyValue().ToString()));
                        
                    }
                    Task.Run(() => {
                        this.AfterCommitSaveListData<T>(entities);
                    });
                }
            }
            catch (SqlException e)
            {
                // ghi log
            }
            finally
            {
                this.DL.UnitOfWork.Dispose();
            }
            return result;
        }

    }
}
