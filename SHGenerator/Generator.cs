using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHGenerator
{
    public class Generator
    {
        private const string ENTER = "\n";
        private const string SPACE12 = "            ";
        private const string SPACE8 = "        ";
        private const string SPACE4 = "    ";
        private const string SPACE2 = "  ";
        private const string SPACE = " ";
        private const string PRIMARYKEY = "PRIMARY KEY";
        private const string NULL = "NULL";
        private const string QUOTATION = "\"";

        public static void GenerateSqlScript(string className,DataTable dt, ref StringBuilder sb)
        {
            sb.Append(SPACE4 + "[DataAttr(" + QUOTATION + dt.TableName + QUOTATION + ")]" + ENTER);
            sb.Append(SPACE4 + "public class "+ className + ENTER + SPACE4 + "{" + ENTER);
            foreach (DataRow dr in dt.Rows)
            {
                string colName = dr["字段名"].ToString();
                string dataType = dr["字段类型"].ToString();
                bool isnull = Convert.ToBoolean(dr["可否为空"]);
                bool iskey = Convert.ToBoolean(dr["是否主键"]);
                bool isidentity = Convert.ToBoolean(dr["自动增长"]);

                string tempType = "";
                sb.Append(SPACE8 + "[DataAttr(" + QUOTATION + colName + QUOTATION);
                if (isidentity)
                    sb.Append(", true");
                sb.Append(")]" + ENTER);
                tempType = ConvertDataType(dataType);
                if (isnull && tempType != "string")
                    tempType = tempType + "?";
                sb.Append(SPACE8 + "public" + SPACE + tempType + SPACE + ConvertPropertyName(colName) + " { get; set; }" + ENTER);
                sb.Append(ENTER);
            }
            sb.Append(SPACE4 + "}");
        }

        private static string ConvertDataType(string dataType)
        {
            string type = "";
            switch (dataType)
            {
                case "varchar":
                case "nvarchar":
                case "text":
                    type = "string";
                    break;
                case "date":
                case "datetime":
                    type = "DateTime";
                    break;
                case "numeric":
                case "money":
                    type = "decimal";
                    break;
                case "tinyint":
                    type = "byte";
                    break;
                case "bigint":
                    type = "Int64";
                    break;
                case "bit":
                    type = "bool";
                    break;
                case "timestamp":
                case "image":
                    type = "byte[]";
                    break;
                case "float":
                    type = "double";
                    break;
                default:
                    type = dataType;
                    break;
            }
            return type;
        }

        static string[] _prefixAttr = new string[] { "vch","dt", "f", "n"};
        private static string ConvertPropertyName(string colName)
        {
            int index = 0;
            foreach(string prefix in _prefixAttr)
            {
                index=colName.IndexOf(prefix);
                if(index==-1)
                {
                    index = 0;
                }
                else
                {
                    index = index + prefix.Length;
                    break;
                }
            }
            string propertyName= colName.Substring(index);
            return propertyName;
        }
    }
}
