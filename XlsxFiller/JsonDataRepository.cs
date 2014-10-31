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
        }

        public void MakeLinkToParent(JsonDataType childType)
        {
            if (false == childType.HasParent) return;

            JsonDataType parentType = myJsonDataTypes.SingleOrDefault(t => t.Name == childType.ParentName);

            foreach (var p in parentType.Entities)
            {
                p.Data[childType.ParentFieldName] = new List<JsonData>();
            }

            foreach (var childEntity in childType.Entities)
            {
                var parentEntity = parentType.GetById(childEntity.ParentId);

                (parentEntity.Data[childType.ParentFieldName] as List<JsonData>).Add(childEntity);
            }
        }

        public IList<JsonDataType> GetAllJsonDataTypes()
        {
            return myJsonDataTypes;
        }

    }
}
