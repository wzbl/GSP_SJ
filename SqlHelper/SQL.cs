using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper
{
    public class SQL
    {
        /// <summary>
        ///  SQL Server连接
        /// </summary>
        private static SqlConnection conn = null;

        public static bool IsConnected()
        {
            if (conn == null)
                return false;
            return conn.State == System.Data.ConnectionState.Open;
        }
        /// <summary>
        ///  Connects to a SQL Server instance
        /// </summary>
        /// <param name="ip">服务地址</param>
        /// <param name="database">数据库名</param>
        /// <param name="user">登录名</param>
        /// <param name="password">密码</param>
        public static void ConnectSqlSever(string ip, string database, string user, string password)
        {
            try
            {
                string connectionString = "Server=" + ip + ";Database=" + database + ";User Id=" + user + ";Password=" + password + ";";
                conn = new SqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("Connected to SQL Server");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQL Server: " + ex.Message);
            }
        }

        public static void ExecuteQuery(string query)
        {
            try
            {
                if (IsConnected())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader[i] + " ");
                        }
                        Console.WriteLine();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
            }
        }

    }
}
