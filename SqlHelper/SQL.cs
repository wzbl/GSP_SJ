using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public static DataTable Excute(string query)
        {
            try
            {
                if (IsConnected())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
                return null;
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

        public static void InsertIMG(string imgfile, string modelfile)
        {
            try
            {
                //将需要存储的图片读取为数据流
                Byte[] btImg = GetByte(imgfile);
                Byte[] btye2 = GetByte(modelfile);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into T_Save(ImgFile,ModelFile) values(@imgfile,@ModelFile)";
                SqlParameter par = new SqlParameter("@imgfile", SqlDbType.Image);
                par.Value = btImg;
                SqlParameter par2 = new SqlParameter("@ModelFile", SqlDbType.Image);
                par2.Value = btye2;
                cmd.Parameters.Add(par);
                cmd.Parameters.Add(par2);
                int t = (int)(cmd.ExecuteNonQuery());
                if (t > 0)
                {
                    Console.WriteLine("插入成功");
                }
            }
            catch (Exception)
            {

            }

        }

        public static byte[] SelectIMG()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from T_Save where id = 1 ";
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                byte[] MyData = (byte[])sdr["ImgFile"];
                return MyData;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static void SelectToDataTable()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from T_Save", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error selecting data: " + ex.Message);
            }
        }

        private static Byte[] GetByte(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Byte[] btye2 = new byte[fs.Length];
            fs.Read(btye2, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return btye2;
        }

        public static void ExecuteProcedure()
        { 
            try
            {
                if (IsConnected())
                {
                    SqlCommand cmd = new SqlCommand("存储名", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", "6005004003024"));
                    cmd.Parameters.Add(new SqlParameter("@ImgFile", "6005004003024"));
                    cmd.Parameters.Add(new SqlParameter("@ModelFile", "6005004003024"));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing procedure: " + ex.Message);
            }
        }

    }
}
