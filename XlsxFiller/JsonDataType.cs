using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlsxFiller
{

    public class JsonDataType
    {
        #region Fields

        public string Name;
        public string ParentName;
        public string ParentFieldName;
        public bool HasParent;
        public JsonDataType Parent;
        public IList<JsonData> Entities;

        #endregion

        #region Behaviour

        public string SerializeEntities()
        {
            string res = JsonDataSerializer.Serialize(Entities);
            return res;
        }

        public void JoinParentEntities()
        {
            if (false == HasParent) return;


        }

        #endregion
        
        #region Helpers

        public JsonData GetById(int id)
        {
            return Entities.SingleOrDefault(e => e.Id == id);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
