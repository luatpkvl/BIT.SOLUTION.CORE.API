using BIT.SOLUTION.DbHepper;
using BIT.SOLUTION.MODEL;
using System.Collections.Generic;
using System.Data;
namespace BIT.SOLUTION.DL
{
    public interface IDLBase
    {
        IUnitOfWork UnitOfWork { set; get; }
        /// <summary>
        /// mo transaction
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Commit Trans
        /// </summary>
        void Commit();
        /// <summary>
        /// 
        /// </summary>
        void RollBack();
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        /// <summary>
        /// thuc hien insert, update,delete
        /// </summary>
        /// <param name="storeProc"></param>
        /// <param name="paramsVal"></param>
        /// <returns></returns>
        bool ExecuteNoneQuery(string storeProc, params string[] paramsVal);
        /// <summary>
        /// tra ve gia tri kieu don
        /// </summary>
        /// <param name="storeProc"></param>
        /// <param name="paramsVal"></param>
        /// <returns></returns>
        object ExecuteScalar(string storeProc, params string[] paramsVal);
        /// <summary>
        /// tra ve doi tuong dataset
        /// </summary>
        /// <param name="storeProc"></param>
        /// <param name="paramsVal"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string storeProc, params string[] paramsVal);
        /// <summary>
        /// tra ve gia tri datarender co the nhieu bang
        /// </summary>
        /// <param name="storeProc"></param>
        /// <param name="paramsVal"></param>
        /// <returns></returns>
        IDataReader GetDataReader(string storeProc, params string[] paramsVal);
        /// <summary>
        /// lay thong tin ban ghi them ID
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        object GetDataCmdTextByID(BaseEntity entity);
        /// <summary>
        /// Lay phan quyen
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> GetPermissionSetting();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyCode"></param>
        void CreateTaskUnitWork(string companyCode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyCode"></param>
        void DestroyTaskUnitWork(string companyCode);
    }
}
