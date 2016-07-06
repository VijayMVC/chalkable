using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

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
                Avg = (double?)announcementDetailsList.Where(x => x.ClassAnnouncementData.ClassAnnouncementTypeRef == classAnnouncementType.Id)
                .Average(x => x.StudentAnnouncements.Average(y => y.NumericScore))
            }).ToList();
        }

        public void CopyClassAnnouncementTypes(int fromClassId, int toClassId, IList<int> typeIds)
        {
            throw new System.NotImplementedException();
        }


        public ClassAnnouncementType GetClassAnnouncementTypeById(int classAnnouncementTypeId)
        {
            return ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId);
        }

        public ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName)
        {
            return string.IsNullOrEmpty(classAnnouncementTypeName)
                     ? null
                     : ChalkableAnnouncementType.All.FirstOrDefault(x => x.Keywords.Split(',').Any(y => classAnnouncementTypeName.ToLower().Contains(y)));

        }

        public ClassAnnouncementType AddClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
           return  ClassAnnouncementTypeStorage.Add(classAnnouncementType);
        }

        public ClassAnnouncementType EditClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
            ClassAnnouncementTypeStorage.Update(classAnnouncementType);
            return GetClassAnnouncementTypeById(classAnnouncementType.Id);
        }

        public void DeleteClassAnnouncmentType(int classAnnouncementTypeId)
        {
            DeleteClassAnnouncmentTypes(new List<int> {classAnnouncementTypeId});
        }
        public void DeleteClassAnnouncmentTypes(IList<int> classAnnouncementTypeIds)
        {
            foreach (var classAnnouncementTypeId in classAnnouncementTypeIds)
            {
                ClassAnnouncementTypeStorage.Delete(classAnnouncementTypeId);
            }
        }
    }
}
