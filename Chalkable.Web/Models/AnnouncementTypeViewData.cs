using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AnnouncementTypeViewData
    {
        public int Id { get; set; }
        public bool IsSystem { get; set; }
        public bool CanCreate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }

        protected AnnouncementTypeViewData(AnnouncementType announcementType)
        {
            Id = announcementType.Id;
            IsSystem = announcementType.IsSystem;
            CanCreate = announcementType.CanCreate;
            Name = announcementType.Name;
            Description = announcementType.Description;
        }

        public static AnnouncementTypeViewData Create(AnnouncementType announcementType)
        {
           return new AnnouncementTypeViewData(announcementType);
        }
        public static IList<AnnouncementTypeViewData> Create(IList<AnnouncementType> announcementTypes)
        {
            return announcementTypes.Select(Create).ToList();
        } 
    }

    public class ClassAnnouncementTypeViewData
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public AnnouncementTypeViewData AnnouncementType { get; set; }

        public static ClassAnnouncementTypeViewData Create(ClassAnnouncementType classAnnouncementType)
        {
            return new ClassAnnouncementTypeViewData
                {
                    Id = classAnnouncementType.Id,
                    ClassId = classAnnouncementType.ClassRef,
                    AnnouncementType = AnnouncementTypeViewData.Create(new AnnouncementType
                        {
                            Id = classAnnouncementType.Id,
                            Description = classAnnouncementType.Description,
                            Gradable = classAnnouncementType.Gradable,
                            Percentage = classAnnouncementType.Percentage,
                            Name = classAnnouncementType.Name,
                        })
                };
        }

        public static IList<ClassAnnouncementTypeViewData> Create(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            return classAnnouncementTypes.Select(Create).ToList();
        } 
    }
}