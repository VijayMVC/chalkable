﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassAnnouncementTypeStorage : BaseDemoIntStorage<ClassAnnouncementType>
    {
        public DemoClassAnnouncementTypeStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<ClassAnnouncementType> GetAll(int classId)
        {
            return data.Where(x => x.Value.ClassRef == classId).Select(x => x.Value).ToList();
        }
    }

    public class DemoClassAnnouncementTypeService : DemoSchoolServiceBase, IClassAnnouncementTypeService
    {
        private DemoClassAnnouncementTypeStorage ClassAnnouncementTypeStorage { get; set; }
        public DemoClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            ClassAnnouncementTypeStorage = new DemoClassAnnouncementTypeStorage();
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(IList<int> classesIds, bool all = true)
        {
            var res = ClassAnnouncementTypeStorage.GetAll().Where(x => classesIds.Contains(x.ClassRef)).ToList();
            if (!all)
                res = res.Where(x => x.Percentage > 0).ToList();
            return res;
        }


        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            return GetClassAnnouncementTypes(new List<int> { classId }, all);
        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            ClassAnnouncementTypeStorage.Add(classAnnouncementTypes);
        }

        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            ClassAnnouncementTypeStorage.Update(classAnnouncementTypes);
        }

        public void Delete(IList<int> ids)
        {
            ClassAnnouncementTypeStorage.Delete(ids);
        }

        public IList<GradedClassAnnouncementType> CalculateAnnouncementTypeAvg(int classId, IList<AnnouncementDetails> announcementDetailsList)
        {
            var classAnnTypes = GetClassAnnouncementTypes(classId, false);
            return classAnnTypes.Select(classAnnouncementType => new GradedClassAnnouncementType
            {
                ClassRef = classAnnouncementType.ClassRef,
                Description = classAnnouncementType.Description,
                Gradable = classAnnouncementType.Gradable,
                Id = classAnnouncementType.Id,
                Name = classAnnouncementType.Name,
                Percentage = classAnnouncementType.Percentage,
                ChalkableAnnouncementTypeRef = classAnnouncementType.ChalkableAnnouncementTypeRef,
                Avg = (double?)announcementDetailsList.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType.Id)
                .Average(x => x.StudentAnnouncements.Average(y => y.NumericScore))
            }).ToList();
        }


        public ClassAnnouncementType GetClassAnnouncementType(int id)
        {
            return ClassAnnouncementTypeStorage.GetById(id);
        }

        public ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName)
        {
            return string.IsNullOrEmpty(classAnnouncementTypeName)
                     ? null
                     : ChalkableAnnouncementType.All.FirstOrDefault(x => x.Keywords.Split(',').Any(y => classAnnouncementTypeName.ToLower().Contains(y)));

        }
    }
}
