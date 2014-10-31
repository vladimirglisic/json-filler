using Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDataFiller
{
    public class Filler
    {
        private JsonDataRepository myRep = new JsonDataRepository();

        public IDictionary<string, string> GetJson(string siteData)
        {
            var document = Workbook.Worksheets(siteData);

            foreach (var w in document)
            {
                var type = WorksheetParser.Parse(w);
                myRep.AddJsonDataType(type);
            }

            foreach (var type in myRep.GetAllJsonDataTypes())
            {
                myRep.MakeLinkToParent(type);
            }

            var json = new Dictionary<string, string>();
            foreach (var type in myRep.GetAllJsonDataTypes().Where(t => false == t.HasParent))
            {
                json[type.Name] = type.SerializeEntities();
            }

            return json;
        }
    }
}
