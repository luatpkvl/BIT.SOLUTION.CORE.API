using BIT.SOLUTION.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BIT.SOLUTION.DbHepper
{
    public interface IUnitOfWork: IDisposable
    {
        string ConnectionString
        {
            set;get;
        }
        MySqlDataBase DB
        {
            get;
        }
        MySqlCommand GetCommandText(string cm, object[] parameterValue);
        /// <summary>
        /// trả về đơn giá trị
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="query"> Câu lệnh dạng text hặc store</param>
        /// <param name="commandType">Kiểu câu lệnh</param>
        /// <returns></returns>
        object ExecuteScalar(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về đơn giá trị
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="query"> Câu lệnh dạng text hặc store</param>
        /// <param name="commandType">Kiểu câu lệnh</param>
        /// <returns></returns>
        object ExecuteScalar(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// vkdien :24/10/2021
        /// trả về số rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        int Execute(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// vkdien :24/10/2021
        /// trả về số rows
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        int Execute(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về giá trị đầu tiên
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        T QueryFistOrDefault<T>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về giá trị đầu tiên
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        T QueryFistOrDefault<T>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về danh sách dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        List<T> Query<T>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về danh sách dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        List<T> Query<T>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// trả về danh sách dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="type">Kiểu dữ liệu trả về</param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        IList Query(string query, Type type, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        Tuple<List<T1>,List<T2>> QueryMultiResult<T1,T2>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        Tuple<List<T1>, List<T2>> QueryMultiResult<T1, T2>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        Tuple<List<T1>, List<T2>,List<T3>> QueryMultiResult<T1, T2,T3>(string query, object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        Tuple<List<T1>, List<T2>, List<T3>> QueryMultiResult<T1, T2, T3>(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Trả về dữ liệu dạng render
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        List<Dictionary<string,object>> QueryListDictionary(string query,  object param = null, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Trả về dữ liệu dạng render
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> QueryListDictionary(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Thực hiện command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        bool ExecuteNoneQueryCmd(MySqlCommand cmd);
        /// <summary>
        /// trả về 1 datarneder
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        MySqlDataReader ExecuteRenderCmd(MySqlCommand cmd);
        /// <summary>
        /// Trả về đơn giá trị
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        object ExecuteScalarCmd(MySqlCommand cmd);
        /// <summary>
        /// tạo command theo store và tham số
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        MySqlCommand  GetStoreProcCommand(string storeName, object[] parameterValue);
        /// <summary>
        /// tạo trấnction
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// tạo đối tượng command
        /// </summary>
        /// <returns></returns>
        MySqlCommand CreateCommand();
        /// <summary>
        /// Thông báo giao dịch thành công
        /// </summary>
        void Commit();
        /// <summary>
        /// Mở connection
        /// </summary>
        void OpenConn();
        /// <summary>
        /// Đóng connection
        /// </summary>
        void CloseConn();
        /// <summary>
        /// Đóng connection
        /// </summary>
        void DisposeConn();
        /// <summary>
        /// Thông báo giao dịch thất bạ, quay lại điểm trước lúc giao dịch
        /// </summary>
        void RollBack();
        /// <summary>
        /// trả vè ds dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        List<T> QueryCustom<T>(string query, object param = null);
        /// <summary>
        /// trả về ds dữ liệu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> QueryMultiResultDictionnary(string query, Type type, Dictionary<string, object> param, CommandType? commandType = CommandType.Text);
        /// <summary>
        /// Trả về danh sách dữ liệu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Tuple<List<Dictionary<string, object>>, List<Dictionary<string, object>>> QueryMultiResultDictionnary(string query, Type type,object param = null, CommandType? commandType = CommandType.Text);
        /// <summary>
        /// hàm adđ giá trị param vào cmd
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramaterName"></param>
        /// <param name="value"></param>
        void AddWithValue(MySqlCommand cmd, string paramaterName, object value);
        /// <summary>
        /// Hàm lấy IDataReader và tự thực hiện đễ lấy dữ liệu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        void ProcessIDataRender(string query, Action<IDataReader> action, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Hàm lấy IDataReader và tự thực hiện đễ lấy dữ liệu
        /// </summary>
        /// <param name="query"></param>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        void ProcessIDataRender(string query, Action<IDataReader> action,object param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Tạo connection
        /// </summary>
        /// <param name="commandCode"></param>
        void CreateSQL(string commandCode);
        /// <summary>
        /// Lưu theo đối tượng entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        bool SaveEntity(object entity, int entityState = 0, string tableName = "", params string[] columsIgnore);
        /// <summary>
        /// Lưu theo đối tượng entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        bool SaveEntity(Dictionary<string, object>  entity, int entityState = 0, string tableName = "", params string[] columsIgnore);
        /// <summary>
        /// Hàm insert theo dataTabse
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="colums"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        int BulkInsertMySQL(DataTable table, string tableName, string colums = "*", int? timeOut = 150);
        /// <summary>
        /// Lấy dữ liệu dạng datatable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        DataTable GetDataTable(string query, Dictionary<string, object> param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Lấy dữ liệu dạng datatable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        DataTable GetDataTable(string query,object param, CommandType? commandType = CommandType.Text, int? timeOut = 150);
        /// <summary>
        /// Hàm insert theo dataTabse
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="colums"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        bool BulkUpdateMySQL(DataTable table, string tableName, string primaryKeyName, string colums = "*", int? timeOut = 150);
        /// <summary>
        /// Lưu đsi tượng theo entity trả về ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        long SaveEntityReturnID(Dictionary<string, object> entity, int entityState, string tableName, params string[] columsIgnore);
        /// <summary>
        /// Lưu đsi tượng theo entity trả về ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityState"></param>
        /// <param name="tableName"></param>
        /// <param name="columsIgnore"></param>
        /// <returns></returns>
        long SaveEntityReturnID(object entity, int entityState, string tableName, params string[] columsIgnore);
    }
}
