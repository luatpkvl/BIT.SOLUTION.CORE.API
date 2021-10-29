using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIT.SOLUTION.Lib
{
    public interface IElasticService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <param name="documentId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        bool InsertOrUpdateDocument(string indexName, string typeName, string documentId, string document);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="documentIDs"></param>
        /// <returns></returns>
        bool DeleteDocument(string indexName, params string[] documentIDs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="columnName"></param>
        /// <param name="documentIDs"></param>
        /// <returns></returns>
        bool DeleteByColumnName(string indexName, string columnName, params string[] documentIDs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        bool Bulk(string messageData);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        bool ExistsType(string indexName, string typeName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <param name="documentId"></param>
        /// <param name="dicValues"></param>
        /// <returns></returns>
        bool UpdateValue(string indexName, string typeName, string documentId, Dictionary<string, object> dicValues);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <param name="dicProperties"></param>
        /// <returns></returns>
        bool AlterMapping(string indexName, string typeName, Dictionary<string, object> dicProperties);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="dicMappings"></param>
        /// <returns></returns>
        bool CreateIndex(string indexName, Dictionary<string, object> dicMappings);


    }
}
