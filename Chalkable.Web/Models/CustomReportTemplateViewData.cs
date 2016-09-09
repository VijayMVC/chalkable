using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class ShortCustomReportTemplateViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconAddress { get; set; }

        protected ShortCustomReportTemplateViewData(CustomReportTemplate template)
        {
            Id = template.Id;
            Name = template.Name;
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
        protected CustomReportTemplateViewData(CustomReportTemplate template) : base(template)
        {
            Layout = template.Layout;
            Style = template.Style;
        }
        public static CustomReportTemplateViewData Create(CustomReportTemplate template)
        {
            return new CustomReportTemplateViewData(template);
        }
    }
}