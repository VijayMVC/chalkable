using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ClassAnnouncementTypeViewData
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
        public bool IsSystem { get; set; }
        public bool CanCreate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public bool Gradable { get; set; }

        protected ClassAnnouncementTypeViewData(ClassAnnouncementType announcementType)
        {
            Id = announcementType.Id;
            ChalkableAnnouncementTypeId = announcementType.ChalkableAnnouncementTypeRef;
            CanCreate = true;
            Name = announcementType.Name;
            Description = announcementType.Description;
            Percentage = announcementType.Percentage;
            Gradable = announcementType.Gradable;
        }

        public static ClassAnnouncementTypeViewData Create(ClassAnnouncementType announcementType)
        {
           return new ClassAnnouncementTypeViewData(announcementType);
        }
        public static IList<ClassAnnouncementTypeViewData> Create(IList<ClassAnnouncementType> announcementTypes)
        {
            return announcementTypes.Select(Create).ToList();
        } 
    }
}