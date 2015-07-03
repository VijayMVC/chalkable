using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StandardImport.Models
{
    public class ABToCCMappingImportModel : CsvContainer
    {
        public const string AB_ID_COLUMN = "GUID";
        public const string CC_ID_COLUMN = "CCSS AB GUID Ref";

        public ABToCCMappingImportModel(byte[] csv)
            : base(csv)
        {
            Mapping = new List<ColumnMappingItem>
                 {
                     new ColumnMappingItem
                         {
                             DestanationColumnName = AB_ID_COLUMN,
                             IsRequired = true,
                             SourceColumnIndex = GetColumnIndex(AB_ID_COLUMN)
                         },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = CC_ID_COLUMN,
                            SourceColumnIndex = GetColumnIndex(CC_ID_COLUMN),
                            IsRequired = true
                        }
                 };
        }
    }
}
