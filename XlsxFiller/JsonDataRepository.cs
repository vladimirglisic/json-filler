using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDataFiller
{
    public class JsonDataRepository
    {
        private IList<JsonDataType> myJsonDataTypes = new List<JsonDataType>();

        public void AddJsonDataType(JsonDataType type)
        {
            myJsonDataTypes.Add(type);

            if (false == type.HasParent) return;

            JsonDataType parentType = myJsonDataTypes.Single(t => t.Name == type.ParentName);

            foreach (var p in parentType.Entities)
            {
                p.Data[type.ParentFieldName] = new List<JsonData>();
            }

            foreach (var e in type.Entities)
            {
                var parent = parentType.GetById(e.ParentId);

                (parent.Data[type.ParentFieldName] as List<JsonData>).Add(e);
            }
        }

        public IList<JsonDataType> GetAllJsonDataTypes()
        {
            return myJsonDataTypes;
        }

    }
}
