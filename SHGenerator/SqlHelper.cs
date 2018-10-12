using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHGenerator
{
    public class SqlHelper : IDisposable
    {
        private static SqlHelper sqlHepler;
        private static SqlConnection conn = new SqlConnection();

        private SqlHelper()
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.AppSettings["MSSQLConnectionString"];
            conn.Open(); // 打开数据库连接
        }

        public static SqlHelper GetSqlHelper()
        {
            if (sqlHepler == null)
            {
                sqlHepler = new SqlHelper();
            }
            return sqlHepler;
        }

        public DataTable ExecuteSql(string sql)
        {
            SqlDataAdapter myda = new SqlDataAdapter(sql, conn); // 实例化适配器
            DataTable dt = new DataTable();
            myda.Fill(dt);
            return dt;
        }

        public DataTable GetAllTableNames()
        {
            string sql = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME ASC;"; // 查询语句
            return ExecuteSql(sql);
        }

        public DataTable GetTableInfo(string tableName)
        {
            string sql = @"SELECT
                       C.name as [字段名],T.name as [字段类型]
                       ,convert(bit,C.IsNullable)  as [可否为空]
                       ,convert(bit,case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=c.id and name in (
                       SELECT name FROM sysindexes WHERE indid in(
                       SELECT indid FROM sysindexkeys WHERE id = c.id AND colid=c.colid))) then 1 else 0 end) 
                       as [是否主键]
                       ,convert(bit,COLUMNPROPERTY(c.id,c.name,'IsIdentity')) as [自动增长]
    
                       FROM syscolumns C
                       INNER JOIN systypes T ON C.xusertype = T.xusertype 
                       left JOIN sys.extended_properties ETP   ON  ETP.major_id = c.id AND ETP.minor_id = C.colid AND ETP.name ='MS_Description' 
                       left join syscomments CM on C.cdefault=CM.id
                       WHERE C.id = object_id('" + tableName + "');"; // 查询语句
            DataTable dt = ExecuteSql(sql);
            dt.TableName = tableName;
            return dt;
        }
        public void Dispose()
        {
            conn.Close();
        }
    }
}
