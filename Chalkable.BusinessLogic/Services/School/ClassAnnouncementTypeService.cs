using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassAnnouncementTypeService
    {
        ClassAnnouncementType GetClassAnnouncementType(int id);
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(IList<int> classesIds, bool all = true); 
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true);
        ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName);

        IList<GradedClassAnnouncementType> CalculateAnnouncementTypeAvg(int classId, IList<AnnouncementDetails> announcementDetailses);
    }

    public class ClassClassAnnouncementTypeService : SisConnectedService, IClassAnnouncementTypeService
    {
        public ClassClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            return GetClassAnnouncementTypes(new List<int> { classId }, all);
        }
        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(IList<int> classesIds, bool all = true)
        {
            var activityCategories = ConnectorLocator.ActivityCategoryConnnector.GetBySectionIds(classesIds);
            var res = BuildClassAnnouncementTypes(activityCategories);
            if (!all)
                res = res.Where(x => x.Percentage > 0).ToList();
            return res;
        }

        public ClassAnnouncementType GetClassAnnouncementType(int id)
        {
            var activityCategory = ConnectorLocator.ActivityCategoryConnnector.GetById(id);
            return BuildClassAnnouncementTypes(new List<ActivityCategory> {activityCategory}).First();
        }

        public ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName)
        {
            return string.IsNullOrEmpty(classAnnouncementTypeName)
                    ? null 
                    : ChalkableAnnouncementType.All.FirstOrDefault(x => x.Keywords.Split(',').Any(y => classAnnouncementTypeName.ToLower().Contains(y)));
        }

        private IList<ClassAnnouncementType> BuildClassAnnouncementTypes(IList<ActivityCategory> activityCategories)
        {
            var announcementTypes = activityCategories.Select(x => new ClassAnnouncementType
            {
                Id = x.Id,
                ClassRef = x.SectionId,
                Description = x.Description,
                Gradable = true,
                Name = x.Name,
                Percentage = (x.Percentage ?? 0)
            }).ToList();
            foreach (var classAnnouncementType in announcementTypes)
            {
                var ct = GetChalkableAnnouncementTypeByAnnTypeName(classAnnouncementType.Name);
                if (ct != null)
                    classAnnouncementType.ChalkableAnnouncementTypeRef = ct.Id;
            }
            return announcementTypes;
        }

        public IList<GradedClassAnnouncementType> CalculateAnnouncementTypeAvg(int classId, IList<AnnouncementDetails> announcementDetailses)
        {
            var classAnnTypes = GetClassAnnouncementTypes(classId);
            var res = new List<GradedClassAnnouncementType>();
            foreach (var classAnnouncementType in classAnnTypes)
            {
                var gradedClassAnnType = new GradedClassAnnouncementType
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
                    };
                res.Add(gradedClassAnnType);
            }
            return res;
        }

    }
}
