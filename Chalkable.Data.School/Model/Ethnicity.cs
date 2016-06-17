using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Ethnicity
    {
        public const string ID_FIELD          = nameof(Id);
        public const string CODE_FIELD        = nameof(Code);
        public const string NAME_FIELD        = nameof(Name);
        public const string DESCRIPTION_FIELD = nameof(Description);
        public const string STATE_CODE_FIELD  = nameof(StateCode);
        public const string SIF_CODE_FIELD    = nameof(SIFCode);
        public const string NCES_CODE_FIELD   = nameof(NCESCode);
        public const string IS_ACTIVE_FIELD   = nameof(IsActive);
        public const string IS_SYSTEM_FIELD   = nameof(IsSystem);

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
