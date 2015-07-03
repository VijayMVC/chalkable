using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementAssignedAttributeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static IList<AnnouncementAssignedAttributeViewData> Create(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            return announcementAttributes.Select(x => new AnnouncementAssignedAttributeViewData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Text
                }).ToList();
        } 
    }
}