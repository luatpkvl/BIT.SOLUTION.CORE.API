
using System;
using System.Data;
using System.Data.SqlClient;

namespace BIT.SOLUTION.Data
{
    public class MySqlDataBase
    {
        private string _connectionString;
        private SqlConnection _connection;
        public MySqlDataBase()
        {

        }
        public MySqlDataBase(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public SqlConnection CreateConnection()
        {
            try
            {
                _connection = new SqlConnection(this._connectionString);
                return _connection;
            }
            catch(Exception e)
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
        public int ExecuteNonQuery(SqlCommand cmd)
        {
            var rs = cmd.ExecuteNonQuery();
            return rs;
        }
        public int ExecuteNonQuery(SqlCommand cmd, SqlTransaction ts)
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
        public SqlDataReader ExcuteRender(SqlCommand cmd)
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
        public SqlDataReader ExcuteRender(SqlCommand cmd, SqlTransaction ts)
        {
            if(ts== null)
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
        public object ExcuteScalar(SqlCommand cmd)
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
        public object ExcuteScalar(SqlCommand cmd, SqlTransaction ts)
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
        public DataSet ExcuteDataSet(SqlCommand cmd)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
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
        public void DiscoverParameters(SqlCommand cmd)
        {
            SqlCommandBuilder.DeriveParameters(cmd);
        }
        /// <summary>
        /// vkdien: create 24/10/2021'
        /// Trả về giá trị cua command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parammeterName"></param>
        /// <returns></returns>
        public object GetParameterValue(SqlCommand cmd, string parammeterName)
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
        public DataSet ExcuteDataSet(SqlCommand cmd, SqlTransaction ts)
        {
            if (ts == null)
            {
                throw new Exception("Transaction iss not null");
            }
            SqlDataAdapter adapter = new SqlDataAdapter();
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
        public SqlCommand GetStoreProcCommand(string storeName, object[] parameterValue)
        {
            SqlCommand cmd = new SqlCommand(storeName, this._connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(cmd);
            int totalParams = cmd.Parameters.Count;
            for (int i = 0; i < totalParams; i++)
            {
                if(parameterValue[i] != null)
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
        public SqlCommand GetStoreProcCommand(SqlTransaction ts,string storeName, object[] parameterValue)
        {
            SqlCommand cmd = new SqlCommand(storeName, this._connection);
            cmd.Transaction = ts;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(cmd);
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
        public SqlCommand GetCommand(string query)
        {
            SqlCommand cmd = new SqlCommand(query, this._connection);
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
        public SqlCommand GetCommand(SqlTransaction ts,string query)
        {
            SqlCommand cmd = new SqlCommand(query, this._connection);
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
            if(this._connection != null)
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
            if(this._connection.State == ConnectionState.Closed)
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
