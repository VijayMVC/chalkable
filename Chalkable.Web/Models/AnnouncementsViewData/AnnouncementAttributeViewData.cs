using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementAttributeViewData
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static IList<AnnouncementAttributeViewData> Create(IList<AnnouncementAttribute> announcementAttributes)
        {
            return announcementAttributes.Select(x => new AnnouncementAttributeViewData
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();
        } 
    }
}