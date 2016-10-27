using System.Collections.Generic;

namespace Chalkable.StandardImport.Models
{
    public class StandardMappingImportModel : CsvContainer
    {
        public string FirstColumn { get; protected set; }
        public string SecondColumn { get; protected set; }

        protected StandardMappingImportModel(byte[] csv, string firstColumn, string secondColumn) : base(csv)
        {
            FirstColumn = firstColumn;
            SecondColumn = secondColumn;

            Mapping = new List<ColumnMappingItem>
                 {
                     new ColumnMappingItem
                         {
                             DestanationColumnName = FirstColumn,
                             IsRequired = true,
                             SourceColumnIndex = GetColumnIndex(FirstColumn)
                         },
                    new ColumnMappingItem
                        {
                            DestanationColumnName = SecondColumn,
                            SourceColumnIndex = GetColumnIndex(SecondColumn),
                            IsRequired = true
                        }
                 };
        }


    }
}
