using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEF
{
    public class DataAttr : Attribute
    {
        private QueryType type;
        public QueryType Type
        {
            get { return type; }
            set { type = value; }
        }
        private string queryStr;//查询语句或者表名列名
        public string QueryStr
        {
            get { return queryStr; }
            set { queryStr = value; }
        }
        private bool isKey;//是否是主键
        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }
        private bool isIdentity;//自动增长
        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }
        public DataAttr(string strSql, QueryType type = QueryType.Name)
        {
            this.queryStr = strSql;
            this.type = type;
        }

        public DataAttr(string strSql, bool isIdentity)
        {
            this.queryStr = strSql;
            this.isIdentity = isIdentity;
        }
        public DataAttr(string strSql, bool isIdentity, bool isKey)
        {
            this.queryStr = strSql;
            this.isIdentity = isIdentity;
            this.isKey = isKey;
        }
    }
    public enum QueryType
    {
        Name,//表示是表名或者字段名
        QuerySql//查询语句
    }
}
