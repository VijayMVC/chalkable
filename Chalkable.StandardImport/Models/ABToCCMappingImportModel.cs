using System.Collections.Generic;

namespace Chalkable.StandardImport.Models
{
    public class ABToCCMappingImportModel : StandardMappingImportModel
    {
        public const string AB_ID_COLUMN = "GUID";
        public const string CC_ID_COLUMN = "CCSS AB GUID Ref";

        public ABToCCMappingImportModel(byte[] csv)
            : base(csv, CC_ID_COLUMN, AB_ID_COLUMN)
        {
        }
    }
}
