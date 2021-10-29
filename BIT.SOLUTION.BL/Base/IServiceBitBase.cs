

using BIT.SOLUTION.Lib;
using BIT.SOLUTION.MODEL;
using System.Collections.Generic;

namespace BIT.SOLUTION.SERVICE
{
    public interface IServiceBitBase : IServiceBase
    {
        ServiceResult SaveData<T>(BaseEntity entity);
        List<ServiceResult> SaveListData<T>(List<T> entities) where T:BaseEntity;
        ServiceResult Delete<T>(BaseEntity entity, string tableName ="", string prmaryKeyName ="", List<T> entities = null);
        T GetDataByID<T>(object id, string column = "*");
        Dictionary<string, object> GetDataByID(object id, string tableName, string column = "*");
        IEnumerable<T> GetDataAll<T>();
        List<Dictionary<string, object>> GetDataAll<T>(string column ="*");
    }
}
