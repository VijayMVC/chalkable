using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementAssignedAttributeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AttributeTypeId { get; set; }
        public string Uuid { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }

        public static IList<AnnouncementAssignedAttributeViewData> Create(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            return announcementAttributes.Select(x => new AnnouncementAssignedAttributeViewData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Text= x.Text,
                    AttributeTypeId = x.AttributeTypeId,
                    Uuid = x.Uuid,
                    VisibleForStudents = x.VisibleForStudents
                    
                }).ToList();
        } 
    }
}