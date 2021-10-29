using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using System.Collections.Generic;

namespace BIT.SOLUTION.DL
{
    public interface IDLCRMBase :IDLBase
    {
        bool DeleteByCmdText(BaseEntity entity, string tableName = "", string primaryKeyName = "");
        /// <summary>
        /// Xoa theo Id
        /// </summary>
        /// <param name="deleteID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool DeleteDataByID(object deleteID, string tableName = "");

        /// <summary>
        /// Kiem tra phai la modlul
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool IsModule(string tableName);
        /// <summary>
        /// Luu data
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Save(BaseEntity entity);
        /// <summary>
        /// Xoa du lieu theo ID va doi tuong
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteData<T>(object id);
        /// <summary>
        ///  Xoa du lieu theo id va table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool DeleteData(object id,string tableName ="");
        /// <summary>
        /// Lay doi tuong theo ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        T GetDataByID<T>(object id, string column = "*");
        /// <summary>
        /// Lay du lieu theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        Dictionary<string,object> GetDataByID(object id, string tableName, string column = "*");
        /// <summary>
        /// Lay data DS
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        List<Dictionary<string,object>> GetListData( string tableName, string column = "*");
        /// <summary>
        /// Lay danh sah
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <param name="enities"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetDataByIDS(string tableName ="", List<long> enities = null, string column ="*");
        /// <summary>
        /// Lay du du lei all theo Doi tuong
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll<T>();


    }
}
