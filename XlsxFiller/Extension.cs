using Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDataFiller
{
    public static class Extension
    {
        public static int CellCount(this Row row)
        {
            int retVal = row.Cells.Any(c => c != null)
                ? row.Cells.Where(c => c != null).Max(c => c.ColumnIndex) + 1
                : 0;

            return retVal;
        }

        public static bool IsComplex(this worksheet worksheet)
        {
            if (worksheet.Rows.Count() < 2) throw new ApplicationException();

            bool isComplex = worksheet.Rows[1].CellCount() > 1;

            return isComplex;
        }
    }
}
