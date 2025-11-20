using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.BaseClass
{
    public class SQLConnect
    {
        private SQLiteConnection _SQLiteConn = null;     //连接对象
        private SQLiteTransaction _SQLiteTrans = null;   //事务对象
        private string _SQLiteConnString = null; //连接字符串
        private bool _AutoCommit = false; //事务自动提交标识

        public string SQLiteConnString
        {
            set { this._SQLiteConnString = value; }
            get { return this._SQLiteConnString; }
        }

        public SQLConnect(string dbPath)
        {
            this._SQLiteConnString = "Data Source=" + dbPath;
            _SQLiteConn = new SQLiteConnection(this._SQLiteConnString);
        }

        public void OpenDB()
        {
            if (_SQLiteConn.State != System.Data.ConnectionState.Open)
                _SQLiteConn.Open();
        }

        /// <summary>
        /// 新建数据库文件
        /// </summary>
        /// <param name="dbPath">数据库文件路径及名称</param>
        /// <returns>新建成功，返回true，否则返回false</returns>
        public Boolean NewDbFile(string dbPath)
        {
            try
            {
                SQLiteConnection.CreateFile(dbPath);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("新建数据库文件".tr() + dbPath + "失败：".tr() + ex.Message);
            }
        }


        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbPath">指定数据库文件</param>
        /// <param name="tableName">表名称</param>
        public void NewTable(string CommandText)
        {
            if (_SQLiteConn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = _SQLiteConn;
                cmd.CommandText = CommandText;
                cmd.ExecuteNonQuery();
            }
            else
            {
                OpenDB();
            }
        }

        public bool Insert(string CommandText)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = _SQLiteConn;
                cmd.CommandText = CommandText;
                int i = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                BrowApp.MessageTip.FloatingTip.ShowError(ex.Message);
                return false;
            }
        }

        public DataTable Search(string CommandText, string table)
        {
            try
            {
                DataTable dt = new DataTable();
                SQLiteCommand cmd = new SQLiteCommand(CommandText, _SQLiteConn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, table);
                return ds.Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
