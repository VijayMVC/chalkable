using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassAnnouncementTypeService : DemoSchoolServiceBase, IClassAnnouncementTypeService
    {
        public DemoClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(IList<int> classesIds, bool all = true)
        {
            var res = Storage.ClassAnnouncementTypeStorage.GetAll().Where(x => classesIds.Contains(x.ClassRef)).ToList();
            if (!all)
                res = res.Where(x => x.Percentage > 0).ToList();
            return res;
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            return GetClassAnnouncementTypes(new List<int> {classId}, all);
        }
        
        public IList<GradedClassAnnouncementType> CalculateAnnouncementTypeAvg(int classId, IList<AnnouncementDetails> announcementDetailses)
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
                Avg = (double?)announcementDetailses.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType.Id)
                .Average(x => x.StudentAnnouncements.Average(y => y.NumericScore))
            }).ToList();
        }


        public ClassAnnouncementType GetClassAnnouncementTypeById(int classAnnouncementTypeId)
        {
            return Storage.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId);
        }


        public ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName)
        {
            return string.IsNullOrEmpty(classAnnouncementTypeName)
                    ? null
                    : ChalkableAnnouncementType.All.FirstOrDefault(x => x.Keywords.Split(',').Any(y => classAnnouncementTypeName.ToLower().Contains(y)));

        }

        public ClassAnnouncementType AddClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
           return  Storage.ClassAnnouncementTypeStorage.Add(classAnnouncementType);
        }

        public ClassAnnouncementType EditClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
            Storage.ClassAnnouncementTypeStorage.Update(classAnnouncementType);
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
                Storage.ClassAnnouncementTypeStorage.Delete(classAnnouncementTypeId);
            }
        }
    }
}
