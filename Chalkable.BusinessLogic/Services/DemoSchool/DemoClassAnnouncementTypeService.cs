using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassAnnouncementTypeService : DemoSchoolServiceBase, IClassAnnouncementTypeService
    {
        public DemoClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            var res = Storage.ClassAnnouncementTypeStorage.GetAll(classId);
            if (!all)
                res = res.Where(x => x.Percentage > 0).ToList();
            return res;

        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            Storage.ClassAnnouncementTypeStorage.Add(classAnnouncementTypes);
        }

        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            Storage.ClassAnnouncementTypeStorage.Edit(classAnnouncementTypes);
        }

        public void Delete(IList<int> ids)
        {
            Storage.ClassAnnouncementTypeStorage.Delete(ids);
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
                Avg = announcementDetailses.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType.Id).Average(x => x.StudentAnnouncements.Average(y => y.NumericScore))
            }).ToList();
        }

    }
}
