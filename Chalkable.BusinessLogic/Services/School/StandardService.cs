using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStandardService
    {
        Standard AddStandard(int id, string name, string description, int standardSubjectId, int? parentStandardId, int? lowerGradeLevelId, int? upperGradeLevelId, bool isActive);
        void AddStandards(IList<Standard> standards);
        void EditStandard(IList<Standard> standards);
        void DeleteStandards(IList<Standard> standards);
        Standard GetStandardById(int id);
        Standard GetStandardByABId(Guid id);
        IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true, bool activeOnly = false);
        IList<Standard> GetStandards(IList<int> standardIds);
        IList<Standard> GetStandards(string filter, int? classId, bool activeOnly = false); 


        IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards); 
        void AddStandardSubjects(IList<StandardSubject> standardSubjects);
        void EditStandardSubjects(IList<StandardSubject> standardSubjects);
        IList<StandardSubject> GetStandardSubjects(int? classId);
        void DeleteStandardSubjects(IList<int> ids);
        void DeleteClassStandards(IList<ClassStandard> classStandards);
        IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId);
        StandardTreePath GetStandardParentsSubTree(int standardId, int? classId);
        IList<Standard> PrepareStandardsCodesData(IList<Standard> standards);
        IList<AnnouncementStandardDetails> PrepareAnnouncementStandardsCodes(IList<AnnouncementStandardDetails> announcementStandards);
    }
    public class StandardService : SchoolServiceBase, IStandardService
    {
        public StandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Standard AddStandard(int id, string name, string description, int standardSubjectId, int? parentStandardId
            , int? lowerGradeLevelId, int? upperGradeLevelId, bool isActive)
        {
            var standard = new Standard
                {
                    Id = id,
                    Description = description,
                    Name = name,
                    IsActive = isActive,
                    LowerGradeLevelRef = lowerGradeLevelId,
                    UpperGradeLevelRef = upperGradeLevelId,
                    ParentStandardRef = parentStandardId,
                    StandardSubjectRef = standardSubjectId
                };
            AddStandards(new List<Standard> {standard});
            return standard;
        }

        public void AddStandards(IList<Standard> standards)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StandardDataAccess(u).Insert(standards));
        }
        
        public Standard GetStandardById(int id)
        {
            return DoRead(u => new StandardDataAccess(u).GetById(id));
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true, bool activeOnly = false)
        {
            using (var uow = Read())
            {
                var res = new StandardDataAccess(uow).GetStandards( new StandardQuery
                    {
                        ClassId = classId,
                        GradeLavelId = gradeLevelId,
                        StandardSubjectId = subjectId,
                        ParentStandardId = parentStandardId,
                        AllStandards = allStandards,
                        ActiveOnly = activeOnly
                    });
                return PrepareStandardsCodesData(res);
            }
        }

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            if(standardSubjects != null && standardSubjects.Count > 0)
                DoUpdate(u => new StandardSubjectDataAccess(u).Insert(standardSubjects));      
        }

        public IList<StandardSubject> GetStandardSubjects(int? classId)
        {
            using (var uow = Read())
            {
                var da = new StandardSubjectDataAccess(uow);
                if (classId.HasValue)
                    return da.GetStandardSubjectByClass(classId.Value);
                return da.GetAll();
            }
        }

        public IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassStandard, int>(u).Insert(classStandards));
            return classStandards;
        }

        public void DeleteClassStandards(IList<ClassStandard> classStandards)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new DataAccessBase<ClassStandard, int>(u).Delete(classStandards));
        }

        public void DeleteStandards(IList<Standard> standards)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new StandardDataAccess(u).Delete(standards));
        }

        public void EditStandard(IList<Standard> standards)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new StandardDataAccess(u).Update(standards));      
        }
        
        public void EditStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new StandardSubjectDataAccess(u).Update(standardSubjects));      
        }


        public void DeleteStandardSubjects(IList<int> ids)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new StandardSubjectDataAccess(u).Delete(ids));      
        }

        
        public Standard GetStandardByABId(Guid id)
        {
            return DoRead(uow => new StandardDataAccess(uow).GetStandardByABId(id));
        }
        
        public IList<Standard> GetStandards(IList<int> standardIds)
        {
            if(standardIds == null || standardIds.Count == 0)
                return new List<Standard>();

            var standards = DoRead(uow => new StandardDataAccess(uow).GetStandardsByIds(standardIds));
            return PrepareStandardsCodesData(standards);
        }

        public IList<Standard> GetStandards(string filter, int? classId, bool activeOnly = false)
        {
            var commonCoreStandards = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService.GetStandards(filter);
            commonCoreStandards = commonCoreStandards.Where(x => x.AcademicBenchmarkId.HasValue).ToList();
            IList<Standard> standards;
            using (var uow = Read())
            {
                var da = new StandardDataAccess(uow);
                standards = da.SearchStandards(filter, activeOnly);
                if (commonCoreStandards.Count > 0)
                   standards = standards.Union(da.GetStandardsByABIds(commonCoreStandards.Select(x => x.AcademicBenchmarkId.Value).ToList())).ToList();
                
                if (classId.HasValue)
                {
                    var standardsByClass = da.GetStandards(new StandardQuery {ClassId = classId, ActiveOnly = activeOnly});
                    standards = standards.Where(s => standardsByClass.Any(s2 => s2.Id == s.Id)).ToList();
                }
            }
            return PrepareStandardsCodesData(standards);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            using (var uow = Read())
            {
                var res = new AnnouncementStandardDataAccess(uow).GetAnnouncementStandardsByAnnId(announcementId);
                return PrepareAnnouncementStandardsCodes(res);
            }
        }

        public StandardTreePath GetStandardParentsSubTree(int standardId, int? classId)
        {
            return DoRead(uow => new StandardDataAccess(uow).GetStandardParentsSubTree(standardId, classId));
        }

        public IList<AnnouncementStandardDetails> PrepareAnnouncementStandardsCodes(IList<AnnouncementStandardDetails> announcementStandards)
        {
            var standards = PrepareStandardsCodesData(announcementStandards.Select(x => x.Standard).ToList());
            foreach (var annStandard in announcementStandards)
            {
                annStandard.Standard = standards.First(s => s.Id == annStandard.StandardRef);
            }
            return announcementStandards;
        } 

        public IList<Standard> PrepareStandardsCodesData(IList<Standard> standards)
        {
            var ccStandardService = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService;
            foreach (var standard in standards.Where(standard => standard.AcademicBenchmarkId.HasValue))
            {
                standard.CCStandardCodes = ccStandardService.GetStandardCodesByABId(standard.AcademicBenchmarkId.Value);
            }
            return standards.OrderBy(x => x.Name).ToList();
        }
    }
}
