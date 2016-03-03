using System.Collections.Generic;

namespace Chalkable.StandardImport.Models
{
    public class AlMappingImportModel : StandardMappingImportModel
    {
        public const string ABTT_GUID_COLUMN = "ABTT GUID";
        public const string AB_STANDARD_COLUMN = "AB Standard GUID";

        public AlMappingImportModel(byte[] csv)
            : base(csv, ABTT_GUID_COLUMN, AB_STANDARD_COLUMN)
        {
        }
    }
}
