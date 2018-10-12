using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEF
{
    public class DbHelper
    {
        #region 私有变量
        private static DbHelper dbHelper;
        private DbConnection _conn;
        private string _connString;
        #endregion

        #region 属性
        public DbConnection Connection
        {
            get { return _conn; }
            set { _conn = value; }
        }
        public string ConnectionString
        {
            get { return _connString; }
            set { _connString = value; }
        }
        #endregion

        #region 虚方法
        public virtual DbConnection OpenDatabase(string connString)
        {
            return null;
        }

        public virtual void CloseDatabase()
        {
            if (_conn != null && _conn.State != ConnectionState.Closed)
                _conn.Close();
        }

        public virtual DbCommand CreateCommand()
        {
            return _conn.CreateCommand();
        }

        public virtual DbCommand GetStoredProcCommand(string storedProcName)
        {
            return null;
        }

        public virtual void AddParameterWithValue(DbCommand command, string parameterName, object value)
        {

        }
        public virtual void AddRangeParameterWithValue(DbCommand command, Dictionary<string, object> parameterdic)
        {

        }
        public virtual void AddInParameter(DbCommand command, string parameterName, object dbType, object value)
        {

        }

        public virtual void AddOutParameter(DbCommand command, string parameterName, object dbType, int size)
        {

        }

        public virtual int ExecuteNonQuery(string sql)
        {
            return -1;
        }

        public virtual int ExecuteNonQuery(CommandType commandType, string sql)
        {
            return -1;
        }

        public virtual int ExecuteNonQuery(DbCommand cmd)
        {
            cmd.Connection = _conn;
            return cmd.ExecuteNonQuery();
        }

        public virtual bool ExecuteNonQuery(string sql, Hashtable ht)
        {
            return false;
        }

        public virtual DataTable GetDataTable(string sql)
        {
            return null;
        }

        public virtual DataSet ExecuteDataSet(DbCommand cmd)
        {
            return null;
        }

        public virtual DataSet ExecuteDataSet(CommandType type, string sql)
        {
            return null;
        }

        public virtual DataTable GetDataTable(string sql, Hashtable ht)
        {
            return null;
        }

        public virtual DataTable GetDataTable(DbCommand command)
        {
            return null;
        }

        public virtual object ExecuteScalar(string sql)
        {
            return null;
        }
        public virtual DbDataReader ExecuteReader(string sql)
        {
            return null;
        }
        #endregion

        #region ORM
        public virtual List<object> SelectData(Type type)
        {
            return null;
        }
        #endregion
    }
}
