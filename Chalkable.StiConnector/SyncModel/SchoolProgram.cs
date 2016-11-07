using System;
using Chalkable.StiConnector.Attributes;

namespace Chalkable.StiConnector.SyncModel
{
    [SisMinVersion("7.3.11.21298")]
    public class SchoolProgram : SyncModel
    {
        public int SchoolProgramID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public override int DefaultOrder => 56;
    }
}
