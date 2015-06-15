using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StandardImport.Models
{
    public class CCStandardImportModel : CsvContainer
    {
        public const string CATEGORY_COLUMN_NAME = "Subject";
        public const string STANDARD_COLUMN_NAME = "Number";
        public const string DESCRIPTION_COLUMN_NAME = "Description";
        public const string ID_COLUMN_NAME = "GUID";
        public const string PARENT_ID_COLUMN_NAME = "Parent GUID";

        public CCStandardImportModel(byte[] csv)
            : base(csv)
        {
            Mapping = new List<ColumnMappingItem>
                {
                    new ColumnMappingItem
                        {
                            DestanationColumnName = CATEGORY_COLUMN_NAME,
                            SourceColumnIndex = GetColumnIndex(CATEGORY_COLUMN_NAME),
                            IsRequired = true
                        },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = STANDARD_COLUMN_NAME,
                            SourceColumnIndex = GetColumnIndex(STANDARD_COLUMN_NAME),
                            IsRequired = true
                        },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = DESCRIPTION_COLUMN_NAME,
                            SourceColumnIndex = GetColumnIndex(DESCRIPTION_COLUMN_NAME),
                            IsRequired = true
                        },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = ID_COLUMN_NAME,
                            SourceColumnIndex = GetColumnIndex(ID_COLUMN_NAME),
                            IsRequired = true
                        },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = PARENT_ID_COLUMN_NAME,
                            SourceColumnIndex = GetColumnIndex(PARENT_ID_COLUMN_NAME),
                            IsRequired = true
                        },
                };

        }
    }
}
