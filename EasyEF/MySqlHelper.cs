using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyEF
{
    public class MySqlHelper : DbHelper, IDisposable
    {
        private static MySqlHelper mySqlHelper;

        public static MySqlHelper GetInstance()
        {
            if (mySqlHelper == null)
                mySqlHelper = new MySqlHelper();
            return mySqlHelper;
        }

        #region 操作数据库的方法
        public override DbConnection OpenDatabase(string connString)
        {
            ConnectionString = connString;
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ApplicationException("没有数据库连接配置项");

            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
            return Connection;
        }

        public override int ExecuteNonQuery(string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 300;
                cmd.Connection = Connection as MySqlConnection;
                cmd.Parameters.Clear();
                return cmd.ExecuteNonQuery();
            }
        }

        public override int ExecuteNonQuery(CommandType commandType, string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = commandType;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 300;
                cmd.Connection = Connection as MySqlConnection;
                cmd.Parameters.Clear();

                return cmd.ExecuteNonQuery();
            }
        }

        public override bool ExecuteNonQuery(string sql, Hashtable ht)
        {
            try
            {
                MySqlCommand command = (Connection as MySqlConnection).CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                if (ht != null)
                {
                    foreach (string paramName in ht.Keys)
                    {
                        object value = ht[paramName];
                        if (value == null)
                            value = DBNull.Value;
                        command.Parameters.AddWithValue(paramName, value);
                    }
                }
                command.ExecuteNonQuery();
                command.Dispose();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override int ExecuteNonQuery(DbCommand cmd)
        {
            try
            {
                MySqlCommand command = cmd as MySqlCommand;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        public override DataTable GetDataTable(string sql, Hashtable ht)
        {
            try
            {
                MySqlCommand command = (Connection as MySqlConnection).CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                if (ht != null)
                {

                    foreach (string paramName in ht.Keys)
                    {
                        command.Parameters.AddWithValue(paramName, ht[paramName]);

                    }
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                command.Dispose();
                return dt;

            }
            catch (Exception e)
            {
                return null;
            }

        }
        public override DataTable GetDataTable(string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 3600;
                cmd.Connection = Connection as MySqlConnection;
                cmd.Parameters.Clear();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
        }

        public override DataTable GetDataTable(DbCommand command)
        {
            DataTable dt = new DataTable();

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command as MySqlCommand))
            {
                adapter.Fill(dt);

            }

            return dt;
        }

        public override DataSet ExecuteDataSet(DbCommand cmd)
        {
            DataSet ds = new DataSet();

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd as MySqlCommand))
            {
                adapter.Fill(ds);

            }

            return ds;
        }

        public override DataSet ExecuteDataSet(CommandType type, string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = type;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 3600;
                cmd.Connection = Connection as MySqlConnection;
                cmd.Parameters.Clear();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
        }

        public override Object ExecuteScalar(string sql)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = 300;
                cmd.Connection = Connection as MySqlConnection;
                cmd.Parameters.Clear();

                return cmd.ExecuteScalar();
            }
        }

        public override DbDataReader ExecuteReader(string sql)
        {
            try
            {
                MySqlCommand command = (Connection as MySqlConnection).CreateCommand();
                command.CommandText = sql;
                return command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                return null;
            }
        }

        public override DbCommand CreateCommand()
        {
            DbCommand cmd = (Connection as MySqlConnection).CreateCommand();

            cmd.CommandTimeout = 300;
            cmd.Connection = Connection;

            return cmd;
        }

        public override DbCommand GetStoredProcCommand(string storedProcName)
        {
            DbCommand cmd = (Connection as MySqlConnection).CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = storedProcName;

            cmd.CommandTimeout = 300;
            cmd.Connection = Connection;

            return cmd;
        }

        public override void AddParameterWithValue(DbCommand command, string parameterName, object value)
        {
            (command as MySqlCommand).Parameters.AddWithValue(parameterName, value);
        }

        public override void AddRangeParameterWithValue(DbCommand command, Dictionary<string, object> parameterdic)
        {
            foreach (string key in parameterdic.Keys)
            {
                object value = parameterdic[key];
                if (value == null)
                    value = DBNull.Value;
                (command as MySqlCommand).Parameters.AddWithValue(key, value);
            }
        }

        public override void AddInParameter(DbCommand command, string parameterName, object dbType, object value)
        {
            (command as MySqlCommand).Parameters.AddWithValue(parameterName, value);
        }

        public override void AddOutParameter(DbCommand command, string parameterName, object dbType, int size)
        {
            (command as MySqlCommand).Parameters.Add(parameterName, (MySqlDbType)dbType, size);
        }
        #endregion

        #region ORM
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="errorStr"></param>
        /// <returns>List<Object></returns>
        public override List<object> SelectData(Type type)
        {
            string strsql = "";
            List<object> listT = new List<object>();
            DataAttr dataAttr = (DataAttr)type.GetCustomAttribute(typeof(DataAttr), false);
            if (dataAttr.Type == QueryType.Name)
                strsql = "select * from [" + dataAttr.QueryStr + "]";
            else
                strsql = dataAttr.QueryStr;
            MySqlDataReader sdr = (MySqlDataReader)ExecuteReader(strsql);
            PropertyInfo[] pinfos = type.GetProperties();
            while (sdr.Read())
            {
                object t = new object();
                foreach (PropertyInfo pinfo in pinfos)
                {
                    DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                    if (pdataAttr == null)
                        continue;
                    for (int i = 0; i < sdr.FieldCount; ++i)
                    {
                        if (sdr[pdataAttr.QueryStr] == DBNull.Value)
                            pinfo.SetValue(t, null);
                        else
                            pinfo.SetValue(t, sdr[pdataAttr.QueryStr]);
                    }
                }
                listT.Add(t);
            }
            return listT;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <returns>返回的对象List</returns>
        public List<T> SelectData<T>()
        {
            string strsql = "";
            Type type = typeof(T);
            List<T> listT = new List<T>();
            DataAttr dataAttr = (DataAttr)type.GetCustomAttribute(typeof(DataAttr), false);
            if (dataAttr.Type == QueryType.Name)
                strsql = "select * from [" + dataAttr.QueryStr + "]";
            else
                strsql = dataAttr.QueryStr;
            MySqlDataReader sdr = (MySqlDataReader)ExecuteReader(strsql);
            PropertyInfo[] pinfos = type.GetProperties();
            while (sdr.Read())
            {
                T t = Activator.CreateInstance<T>();
                foreach (PropertyInfo pinfo in pinfos)
                {
                    DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                    if (pdataAttr == null)
                        continue;
                    for (int i = 0; i < sdr.FieldCount; ++i)
                    {

                        if (sdr[pdataAttr.QueryStr] == DBNull.Value)
                            pinfo.SetValue(t, null);
                        else
                            pinfo.SetValue(t, sdr[pdataAttr.QueryStr]);
                    }
                }
                listT.Add(t);
            }
            return listT;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <returns>返回的对象List</returns>
        public List<T> SelectData<T>(SearchCondition searchCondition)
        {
            string strsql = "";
            Type type = typeof(T);
            List<T> listT = new List<T>();
            DataAttr dataAttr = (DataAttr)type.GetCustomAttribute(typeof(DataAttr), false);
            if (dataAttr.Type == QueryType.Name)
                strsql = "select * from [" + dataAttr.QueryStr + "]" + searchCondition.BuildConditionSql();
            else
                strsql = dataAttr.QueryStr + searchCondition.BuildConditionSql();
            MySqlDataReader sdr = (MySqlDataReader)ExecuteReader(strsql);
            PropertyInfo[] pinfos = type.GetProperties();
            while (sdr.Read())
            {
                T t = Activator.CreateInstance<T>();
                foreach (PropertyInfo pinfo in pinfos)
                {
                    DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                    if (pdataAttr == null)
                        continue;
                    for (int i = 0; i < sdr.FieldCount; ++i)
                    {

                        if (sdr[pdataAttr.QueryStr] == DBNull.Value)
                            pinfo.SetValue(t, null);
                        else
                            pinfo.SetValue(t, sdr[pdataAttr.QueryStr]);
                    }
                }
                listT.Add(t);
            }
            return listT;
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lisT">要插入的对象List</param>
        /// <returns></returns>
        public bool InsertData<T>(List<T> lisT)
        {
            foreach(T t in lisT)
            {
               if(!InsertData<T>(t))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 逐条插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool InsertData<T>(T t)
        {
            Type type = typeof(T);
            string strsql = GeneratorInsertSql(type);
            if (strsql == null)
            {
                return false;
            }
            PropertyInfo[] pinfos = type.GetProperties();
            Hashtable hashTable = new Hashtable();
            foreach (PropertyInfo pinfo in pinfos)
            {
                DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                if (pdataAttr == null)
                    continue;
                if (pdataAttr.IsIdentity)
                    continue;
                string para = "@" + pdataAttr.QueryStr;
                object value = pinfo.GetValue(t);
                hashTable.Add(para, value);
            }
            return ExecuteNonQuery(strsql, hashTable);
        }
        /// <summary>
        /// 将List转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listT"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable<T>(List<T> listT)
        {
            DataTable dtSource = new DataTable();
            Type type = typeof(T);
            PropertyInfo[] pinfos = type.GetProperties();
            foreach (PropertyInfo pinfo in pinfos)
            {
                DataAttr dataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                if (dataAttr == null)
                {
                    continue; ;
                }
                DataColumn dc = new DataColumn(dataAttr.QueryStr, pinfo.PropertyType);
                dtSource.Columns.Add(dc);
            }
            foreach (T t in listT)
            {
                object[] objs = new object[dtSource.Columns.Count];
                for (int i = 0; i < dtSource.Columns.Count; ++i)
                {
                    DataColumn dc = dtSource.Columns[i];
                    foreach (PropertyInfo pinfo in pinfos)
                    {
                        DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                        if (pdataAttr == null)
                            continue;
                        if (pdataAttr.QueryStr == dc.ColumnName)
                            objs[i] = pinfo.GetValue(t);
                    }
                }
                dtSource.Rows.Add(objs);
            }
            return dtSource;
        }
        /// <summary>
        /// 将DataRow 转 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ConvertDataRowToT<T>(DataRow dr)
        {
            Type type = typeof(T);
            T t = (T)Activator.CreateInstance(type);
            PropertyInfo[] pInfos = type.GetProperties();
            foreach (DataColumn dc in dr.Table.Columns)
            {
                foreach (PropertyInfo info in pInfos)
                {
                    DataAttr infoAttr = (DataAttr)info.GetCustomAttribute(typeof(DataAttr), false);
                    if (infoAttr == null)
                        continue;
                    if (infoAttr.QueryStr.Equals(dc.ColumnName))
                    {
                        if (info.PropertyType.Name.Equals("String"))
                        {
                            if (dr[dc.ColumnName] == DBNull.Value)
                                info.SetValue(t, "");
                            else
                                info.SetValue(t, dr[dc.ColumnName]);
                        }
                        else
                            info.SetValue(t, dr[dc.ColumnName]);
                    }
                }
            }
            return t;
        }
        /// <summary>
        /// 将DataTable 转 List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> tList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T t = ConvertDataRowToT<T>(dr);
                tList.Add(t);
            }
            return tList;
        }
        /// <summary>
        /// 生成插入的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        private static string GeneratorInsertSql(Type type)
        {
            StringBuilder strb = new StringBuilder();
            DataAttr dataAttr = (DataAttr)type.GetCustomAttribute(typeof(DataAttr), false);
            if (dataAttr == null)
            {
                return null;
            }
            if (dataAttr.Type == QueryType.Name)
            {
                strb.Append("insert into [" + dataAttr.QueryStr + "] (");
            }
            PropertyInfo[] pinfos = type.GetProperties();
            foreach (PropertyInfo pinfo in pinfos)
            {
                DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                if (pdataAttr == null)
                    continue;
                if (pdataAttr.IsIdentity)
                    continue;
                strb.Append(pdataAttr.QueryStr + ",");
            }
            strb.Remove(strb.Length - 1, 1);
            strb.Append(") values (");
            foreach (PropertyInfo pinfo in pinfos)
            {
                DataAttr pdataAttr = (DataAttr)pinfo.GetCustomAttribute(typeof(DataAttr), false);
                if (pdataAttr == null)
                    continue;
                if (pdataAttr.IsIdentity)
                    continue;
                strb.Append("@" + pdataAttr.QueryStr + ",");
            }
            strb.Remove(strb.Length - 1, 1);
            strb.Append(")");
            return strb.ToString();
        }
        /// <summary>
        /// 更新根据Key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateByID(object data)
        {
            Type type = data.GetType();
            DataAttr dataAttr = (DataAttr)type.GetCustomAttribute(typeof(DataAttr), false);
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> keyDic = new Dictionary<string, object>();
            sb.Append("UPDATE  ");
            sb.Append(dataAttr.QueryStr);
            sb.Append(" Set ");
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                DataAttr infoAttr = (DataAttr)info.GetCustomAttribute(typeof(DataAttr), false);
                if (infoAttr == null)
                    continue;
                if (infoAttr.IsKey)
                {
                    keyDic.Add(infoAttr.QueryStr, info.GetValue(data));
                }
                else
                {
                    sb.Append(infoAttr.QueryStr + " =@" + infoAttr.QueryStr + " ,");
                }
            }
            sb.Remove(sb.Length - 1, 1);//移除 多余的 ","
            sb.Append(" WHERE ");
            foreach (string key in keyDic.Keys)
            {
                sb.Append(key + " =@" + key + " AND ");
            }
            sb.Remove(sb.Length - 5, 5);//移除多余的 'AND'

            Hashtable hashTable = new Hashtable();
            for (int i = 0; i < propertyInfos.Length; ++i)
            {
                PropertyInfo info = propertyInfos[i];
                DataAttr infoAttr = (DataAttr)info.GetCustomAttribute(typeof(DataAttr), false);
                if (infoAttr == null)
                    continue;
                string strPara = "@" + infoAttr.QueryStr;
                object value = info.GetValue(data);
                hashTable.Add(strPara, value);
            }
            return ExecuteNonQuery(sb.ToString(), hashTable);
        }
        #endregion

        #region IDisposable Support
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
