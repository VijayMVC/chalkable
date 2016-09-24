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
        public Guid? HeaderRef { get; set; }
        public Guid? FooterRef { get; set; }
        public int Type { get; set; }

        [NotDbFieldAttr]
        public CustomReportTemplate Header { get; set; }

        [NotDbFieldAttr]
        public CustomReportTemplate Footer { get; set; }

    }

    public enum TemplateType
    {
        Body = 1,
        Header = 2,
        Footer = 3
    }
}
