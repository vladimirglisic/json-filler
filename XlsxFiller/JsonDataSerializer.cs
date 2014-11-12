using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDataFiller
{
    public class JsonDataSerializer
    {
        public static string Serialize(IList<JsonData> entities)
        {
            string res;

            if (entities.Count > 0 && entities.First().IsDict())
            {
                res = SerializeGroup(entities);
            }
            else
            {
                res = SerializeEntities(entities);
            }

            return res;
        }

        private static string SerializeGroup(IList<JsonData> entitites)
        {
            StringBuilder sb = new StringBuilder();

            var groups = entitites.GroupBy(e => e.ParentId); // gruoup by pid

            sb.AppendLine("{");
            foreach (var g in groups)
            {
                bool isLastGroup = (g.Key == groups.Last().Key);
                string comma = isLastGroup ? "" : ",";

                sb.AppendFormat("\"{0}\":", g.Key);
                sb.Append(SerializeEntities(g.ToList()));
                sb.AppendLine(comma);
            }
            sb.AppendLine("}");

            string res = sb.ToString();
            return res;
        }

        private static string SerializeEntities(IList<JsonData> entities)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            foreach (var e in entities)
            {
                bool isLastEntity = entities.Last() == e;
                string entityComma = isLastEntity ? "" : ",";
                sb.Append(e.Serialize());
                sb.Append(entityComma);
                if (false == e.IsSimple()) sb.AppendLine();
            }

            sb.Append("]");
            string res = sb.ToString();
            return res;
        }
    }
}
