using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class ShortCustomReportTemplateViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public Guid? HeaderId { get; set; }
        public Guid? FooterId { get; set; }
        public bool HasHeader { get; set; }
        public bool HasFooter { get; set; }

        protected ShortCustomReportTemplateViewData(CustomReportTemplate template)
        {
            Id = template.Id;
            Name = template.Name;
            Type = template.Type;
            HeaderId = template.HeaderRef;
            FooterId = template.FooterRef;
            HasHeader = template.HeaderRef.HasValue;
            HasFooter = template.FooterRef.HasValue;
        }
        public static IList<ShortCustomReportTemplateViewData> Create(IList<CustomReportTemplate> templates)
        {
            return templates.Select(t => new ShortCustomReportTemplateViewData(t)).ToList();
        }
    }


    public class CustomReportTemplateViewData : ShortCustomReportTemplateViewData
    {
        public string Layout { get; set; }
        public string Style { get; set; }
        public ShortCustomReportTemplateViewData Header { get; set; }
        public ShortCustomReportTemplateViewData Footer { get; set; }

        protected CustomReportTemplateViewData(CustomReportTemplate template) : base(template)
        {
            Layout = template.Layout;
            Style = template.Style;
            if (template.Header != null)
                Header = Create(template.Header);
            if (template.Footer != null)
                Footer = Create(template.Footer);
        }
        public static CustomReportTemplateViewData Create(CustomReportTemplate template)
        {
            return new CustomReportTemplateViewData(template);
        }
    }
}