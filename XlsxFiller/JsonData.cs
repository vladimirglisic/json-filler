using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDataFiller
{
    public class JsonData
    {
        #region Fields

        public const string ID = "id";
        public const string PARENT_ID = "#pid#";
        public const string DATA = "#data#";
        public const string TITLE = "title";

        #endregion

        #region Properties

        public JsonDataType Type;

        public IDictionary<string, object> Data;

        public int Id
        {
            get { return Data.ContainsKey(ID) ? Convert.ToInt32(Data[ID]) : 0; }
        }

        public int ParentId
        {
            get { return Data.ContainsKey(PARENT_ID) ? Convert.ToInt32(Data[PARENT_ID]) : 0; }
        }

        #endregion

        #region Behavior

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(IsSimple() ? "" : "{\r\n");
            foreach (var k in Data)
            {
                bool isLastField = Data.Last().Key == k.Key;
                string keyComma = isLastField ? "" : ",";

                if (k.Value is IList<JsonData>)                                                     // list of entities
                {
                    string value = JsonDataSerializer.Serialize(k.Value as IList<JsonData>);
                    sb.AppendLine(string.Format("\"{0}\":{1}{2}", k.Key, value, keyComma));
                }
                else if (IsSimple())                                                              // simple entity
                {
                    if (k.Key == JsonData.DATA)
                        sb.AppendFormat("\"{0}\"", k.Value);
                }
                else if (k.Key == JsonData.PARENT_ID)
                {
                    if (IsDict() || Type.HasParent)
                        continue;
                }
                else                                                                                // regular
                {
                    sb.AppendLine(string.Format("\"{0}\":\"{1}\"{2}", k.Key, k.Value, keyComma));
                }
            }
            sb.Append(IsSimple() ? "" : "}");

            string res = sb.ToString();
            return res;
        }

        #endregion

        #region Helpers

        public object GetByKey(string key)
        {
            return Data[key];
        }

        public bool IsSimple()
        {
            return Data.ContainsKey(DATA);
        }

        public bool IsDict()
        {
            bool isDict = Data.ContainsKey(PARENT_ID);

            isDict = isDict && !Type.HasParent;

            return isDict;
        }

        public override string ToString()
        {
            if (true == Data.ContainsKey(DATA))
                return string.Format("{0}, {1}", Data[DATA].ToString(), Type.Name);
            else if (true == Data.ContainsKey(TITLE))
                return string.Format("{0}, {1}", Data[TITLE].ToString(), Type.Name);
            else
                return Type.Name;
        }

        #endregion
    }
}
