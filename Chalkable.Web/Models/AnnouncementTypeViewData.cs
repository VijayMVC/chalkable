using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AnnouncementTypeViewData
    {
        public int AnnouncementTypeId { get; set; }
        public bool IsSystem { get; set; }
        public bool CanCreate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }

        protected AnnouncementTypeViewData(AnnouncementType announcementType)
        {
            AnnouncementTypeId = announcementType.Id;
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

    public class ClassAnnouncementTypeViewData : AnnouncementTypeViewData
    {
        public int Id { get; set; }
        public int ClassId { get; set; }

        protected ClassAnnouncementTypeViewData(ClassAnnouncementType classAnnouncementType)
            : base(new AnnouncementType
            {
                Id = classAnnouncementType.AnnouncementTypeRef,
                Description = classAnnouncementType.Description,
                Gradable = classAnnouncementType.Gradable,
                Percentage = classAnnouncementType.Percentage,
                Name = classAnnouncementType.Name,
                CanCreate = true
            })
        {
            Id = classAnnouncementType.Id;
            ClassId = classAnnouncementType.ClassRef;
        }


        public static ClassAnnouncementTypeViewData Create(ClassAnnouncementType classAnnouncementType)
        {
            return new ClassAnnouncementTypeViewData(classAnnouncementType);
        }

        public static IList<ClassAnnouncementTypeViewData> Create(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            return classAnnouncementTypes.Select(Create).ToList();
        } 
    }
}