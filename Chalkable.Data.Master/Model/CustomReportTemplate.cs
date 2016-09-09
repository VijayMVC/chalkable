using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class CustomReportTemplate
    {

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }
        public string Layout { get; set; } 
    }
}
