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

        public static IList<ShortCustomReportTemplateViewData> Create(IList<CustomReportTemplate> templates)
        {
            return templates.Select(t => new ShortCustomReportTemplateViewData
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }
    }
}