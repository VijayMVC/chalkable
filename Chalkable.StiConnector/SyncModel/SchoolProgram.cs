﻿using System;

namespace Chalkable.StiConnector.SyncModel
{
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
