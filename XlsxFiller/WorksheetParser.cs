using Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlsxFiller
{
    public class WorksheetParser
    {
        public static JsonDataType Parse(worksheet worksheet)
        {
            JsonDataType type = WorksheetParser.ParseHeader(worksheet);
            var data = WorksheetParser.ParseData(worksheet);

            IList<JsonData> entities = data
                .Select(d => new JsonData
                {
                    Type = type,
                    Data = d
                })
                .ToList();

            type.Entities = entities;

            return type;
        }

        private static JsonDataType ParseHeader(worksheet worksheet)
        {
            if (worksheet.Rows.Count() < 1) return null;

            var infoRow = worksheet.Rows[0];

            string name = string.Join("_", infoRow.Cells.ToList().GetRange(0, infoRow.CellCount()).Select(c => c.Text));

            bool hasParent = (infoRow.CellCount() > 1);

            string parentName = (true == hasParent)
                ? string.Join("_", infoRow.Cells.ToList().GetRange(0, infoRow.CellCount() - 1).Select(c => c.Text))
                : string.Empty;

            string fieldName = (true == hasParent)
                ? infoRow.Cells[infoRow.CellCount() - 1].Text
                : string.Empty;

            var type = new JsonDataType()
            {
                Name = name,
                ParentName = parentName,
                ParentFieldName = fieldName,
                HasParent = hasParent
            };

            return type;
        }

        private static IList<IDictionary<string, object>> ParseData(worksheet worksheet)
        {
            if (worksheet.Rows.Count() < 1) return null;

            if (worksheet.Rows.Count() == 1) return new List<IDictionary<string, object>>();

            var keyRow = worksheet.Rows[1];

            var data = new List<IDictionary<string, object>>();

            for (int i = 2; i < worksheet.Rows.Count(); i++)
            {
                var currRow = worksheet.Rows[i];
                var currDict = new Dictionary<string, object>();

                for (int j = 0; j < keyRow.CellCount(); j++)
                {
                    if (currRow.Cells[j] == null)
                        currDict[keyRow.Cells[j].Text] = null;
                    else if (true == currRow.Cells[j].IsAmount)
                        currDict[keyRow.Cells[j].Text] = currRow.Cells[j].Value;
                    else
                        currDict[keyRow.Cells[j].Text] = currRow.Cells[j].Text;
                }

                data.Add(currDict);
            }
            return data;
        }
    }
}
