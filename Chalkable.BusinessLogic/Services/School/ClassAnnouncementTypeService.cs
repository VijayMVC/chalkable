using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassAnnouncementTypeService
    {
        ClassAnnouncementType AddClassAnnouncmentType(ClassAnnouncementType classAnnouncementType);
        ClassAnnouncementType EditClassAnnouncmentType(ClassAnnouncementType classAnnouncementType);
        void DeleteClassAnnouncmentType(int classAnnouncementTypeId);
        void DeleteClassAnnouncmentTypes(IList<int> classAnnouncementTypeIds);

        ClassAnnouncementType GetClassAnnouncementTypeById(int classAnnouncementTypeId);
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(IList<int> classesIds, bool all = true); 
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true);
        ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName);

        IList<GradedClassAnnouncementType> CalculateAnnouncementTypeAvg(int classId, IList<AnnouncementDetails> announcementDetailses);
        void CopyClassAnnouncementTypes(int fromClassId, int toClassId, IList<int> typeIds);
    }

    public class ClassClassAnnouncementTypeService : SisConnectedService, IClassAnnouncementTypeService
    {
        public ClassClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        public ClassAnnouncementType AddClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
            var activityCategory = BuildActivityCategory(classAnnouncementType);
            activityCategory = ConnectorLocator.ActivityCategoryConnnector.Add(activityCategory);
            return BuildClassAnnouncementType(activityCategory);
        }

        public ClassAnnouncementType EditClassAnnouncmentType(ClassAnnouncementType classAnnouncementType)
        {
            var activityCategory = BuildActivityCategory(classAnnouncementType);
            ConnectorLocator.ActivityCategoryConnnector.Update(activityCategory.Id, activityCategory);
            return BuildClassAnnouncementType(activityCategory);
        }

        public void DeleteClassAnnouncmentType(int classAnnouncementTypeId)
        {
            DeleteClassAnnouncmentTypes(new List<int>{classAnnouncementTypeId});
        }
        public void DeleteClassAnnouncmentTypes(IList<int> classAnnouncementTypeIds)
        {
            foreach (var classAnnouncementTypeId in classAnnouncementTypeIds)
            {
                ConnectorLocator.ActivityCategoryConnnector.Delete(classAnnouncementTypeId);
            }
        }

        public ClassAnnouncementType GetClassAnnouncementTypeById(int classAnnouncementTypeId)
        {
            var activityCategory = ConnectorLocator.ActivityCategoryConnnector.GetById(classAnnouncementTypeId);
            if (activityCategory == null)
                return null;
            return BuildClassAnnouncementTypes(new List<ActivityCategory> { activityCategory }).First();
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

        public ChalkableAnnouncementType GetChalkableAnnouncementTypeByAnnTypeName(string classAnnouncementTypeName)
        {
            return string.IsNullOrEmpty(classAnnouncementTypeName)
                    ? null 
                    : ChalkableAnnouncementType.All.FirstOrDefault(x => x.Keywords.Split(',').Any(y => classAnnouncementTypeName.ToLower().Contains(y)));
        }
        

        private ActivityCategory BuildActivityCategory(ClassAnnouncementType classAnnouncementType)
        {
            return new ActivityCategory
            {
                Id = classAnnouncementType.Id,
                Name = classAnnouncementType.Name,
                Description = classAnnouncementType.Description,
                HighScoresToDrop = classAnnouncementType.HighScoresToDrop,
                LowScoresToDrop = classAnnouncementType.LowScoresToDrop,
                Percentage = classAnnouncementType.Percentage,
                SectionId = classAnnouncementType.ClassRef,
                IsSystem = classAnnouncementType.IsSystem
            };
        }

        private ClassAnnouncementType BuildClassAnnouncementType(ActivityCategory activityCategory)
        {
            var res = new ClassAnnouncementType
                {
                    Id = activityCategory.Id,
                    ClassRef = activityCategory.SectionId,
                    Description = activityCategory.Description,
                    Gradable = true,
                    Name = activityCategory.Name,
                    Percentage = (activityCategory.Percentage ?? 0),
                    LowScoresToDrop = activityCategory.LowScoresToDrop,
                    HighScoresToDrop = activityCategory.HighScoresToDrop,
                    IsSystem = activityCategory.IsSystem
                };
            var ct = GetChalkableAnnouncementTypeByAnnTypeName(res.Name);
            if (ct != null)
                res.ChalkableAnnouncementTypeRef = ct.Id;
            return res;
        }

        private IList<ClassAnnouncementType> BuildClassAnnouncementTypes(IEnumerable<ActivityCategory> activityCategories)
        {
            return activityCategories != null 
                ? activityCategories.Select(BuildClassAnnouncementType).ToList() 
                : new List<ClassAnnouncementType>();
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
                        Avg = (double?)announcementDetailses.Where(x =>x.ClassAnnouncementData != null && x.ClassAnnouncementData.ClassAnnouncementTypeRef == classAnnouncementType.Id)
                                                   .Average(x => x.StudentAnnouncements.Average(y => y.NumericScore))
                    };
                res.Add(gradedClassAnnType);
            }
            return res;
        }

        public void CopyClassAnnouncementTypes(int fromClassId, int toClassId, IList<int> typeIds)
        {
            var copyOption = new ActivityCategoryCopyOption { CategoryIds = typeIds, CopyToSectionIds = new [] {toClassId}};
            ConnectorLocator.ActivityCategoryConnnector.CopyCategories(fromClassId, copyOption);
        }
    }
}
