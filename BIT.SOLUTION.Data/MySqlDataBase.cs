using MySql.Data.MySqlClient;
using System;
using System.Data;
namespace BIT.SOLUTION.Data
{
    public class MySqlDataBase
    {
        private string _connectionString;
        private MySqlConnection _connection;
        public MySqlDataBase()
        {

        }
        public MySqlDataBase(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public MySqlConnection CreateConnection()
        {
            try
            {
                _connection = new MySqlConnection(this._connectionString);
                return _connection;
            }
            catch (Exception e)
            {
                throw new Exception($"ConnectDb Failse !: {e.Message}");
            }
        }
        /// <summary>
        /// THực hiện hành động thêm sửa xóa
        /// </summary>
        /// vkdien: create 24/10/2021
        /// <param name="cmd"></param>
        /// <returns> 0 là thành công  </returns>
        public int ExecuteNonQuery(MySqlCommand cmd)
        {
            var rs = cmd.ExecuteNonQuery();
            return rs;
        }
        public int ExecuteNonQuery(MySqlCommand cmd, MySqlTransaction ts)
        {
            if (ts == null)
            {
                throw new Exception("Transaction iss not null");
            }
            cmd.Transaction = ts;
            return cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// Select trong dataBase
        /// </summary>
        /// vkdien: create 24/10/2021
        /// <param name="cmd"></param>
        /// <returns></returns>
        public MySqlDataReader ExcuteRender(MySqlCommand cmd)
        {
            return cmd.ExecuteReader();
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Select trong dataBase
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ts"> Transaction thực hiện câu lệnh</param>
        /// <returns></returns>
        public MySqlDataReader ExcuteRender(MySqlCommand cmd, MySqlTransaction ts)
        {
            if (ts == null)
            {
                throw new Exception("Transaction iss not null");
            }
            cmd.Transaction = ts;
            return cmd.ExecuteReader();
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Select trong dataBase trả về đơn giá trị
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ts"> Transaction thực hiện câu lệnh</param>
        /// <returns></returns>
        /// 
        public object ExcuteScalar(MySqlCommand cmd)
        {
            return cmd.ExecuteScalar();
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Select trong dataBase trả về đơn giá trị
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ts"> Transaction thực hiện câu lệnh</param>
        /// <returns></returns>
        /// 
        public object ExcuteScalar(MySqlCommand cmd, MySqlTransaction ts)
        {
            if (ts == null)
            {
                throw new Exception("Transaction iss not null");
            }
            cmd.Transaction = ts;
            return cmd.ExecuteScalar();
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Select trong dataBase trả về dataset
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataSet ExcuteDataSet(MySqlCommand cmd)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Lấy danh sách tham số của Store
        /// </summary>
        /// <param name="cmd"></param>
        public void DiscoverParameters(MySqlCommand cmd)
        {
            MySqlCommandBuilder.DeriveParameters(cmd);
        }
        /// <summary>
        /// vkdien: create 24/10/2021'
        /// Trả về giá trị cua command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parammeterName"></param>
        /// <returns></returns>
        public object GetParameterValue(MySqlCommand cmd, string parammeterName)
        {
            return cmd.Parameters[parammeterName].Value;
        }
        /// <summary>
        /// vkdien: create 24/10/2021
        /// Select trong dataBase trả về dataset
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ts"> Transaction thực hiện câu lệnh</param>
        /// <returns></returns>
        public DataSet ExcuteDataSet(MySqlCommand cmd, MySqlTransaction ts)
        {
            if (ts == null)
            {
                throw new Exception("Transaction iss not null");
            }
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
        /// <summary>
        /// Hành động select trong db trả về đối tượng thực thi câu lệnh
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public MySqlCommand GetStoreProcCommand(string storeName, object[] parameterValue)
        {
            MySqlCommand cmd = new MySqlCommand(storeName, this._connection);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(cmd);
            int totalParams = cmd.Parameters.Count;
            for (int i = 0; i < totalParams; i++)
            {
                if (parameterValue[i] != null)
                {
                    cmd.Parameters[i].Value = parameterValue[i];
                }
                else
                {
                    cmd.Parameters[i].Value = DBNull.Value;
                }
            }
            return cmd;
        }
        /// <summary>
        ///  Hành động select trong db trả về đối tượng thực thi câu lệnh
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="storeName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public MySqlCommand GetStoreProcCommand(MySqlTransaction ts, string storeName, object[] parameterValue)
        {
            MySqlCommand cmd = new MySqlCommand(storeName, this._connection);
            cmd.Transaction = ts;
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(cmd);
            int totalParams = cmd.Parameters.Count;
            for (int i = 0; i < totalParams; i++)
            {
                if (parameterValue[i] != null)
                {
                    cmd.Parameters[i].Value = parameterValue[i];
                }
                else
                {
                    cmd.Parameters[i].Value = DBNull.Value;
                }
            }
            return cmd;
        }
        /// <summary>
        /// Trả về đối tượng thực thi câu lệnh
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public MySqlCommand GetCommand(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, this._connection);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }
        /// <summary>
        ///  Trả về đối tượng thực thi câu lệnh
        /// vkdien :24/10/2021
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public MySqlCommand GetCommand(MySqlTransaction ts, string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, this._connection);
            cmd.Transaction = ts;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }
        /// <summary>
        /// Giải phóng connection
        /// vkdien :24/10/2021
        /// </summary>
        public void Dispose()
        {
            if (this._connection != null)
            {
                this._connection.Dispose();
                this._connection = null;
            }
        }
        /// <summary>
        /// Mở cnn
        /// vkdien :24/10/2021
        /// </summary>
        public void OppenConn()
        {
            if (this._connection.State == ConnectionState.Closed)
            {
                this._connection.Open();
            }
        }
        /// <summary>
        /// Đóng cnn
        /// vkdien :24/10/2021
        /// </summary>
        public void CloseConn()
        {
            if (this._connection.State == ConnectionState.Open)
            {
                this._connection.Close();
            }
        }
        ~MySqlDataBase()
        {
            Dispose();
        }
    }
}
