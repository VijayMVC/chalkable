using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
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
        IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true);
        
        IList<Standard> GetStandards(IList<int> standardIds);
        IList<Standard> GetStandards(string filter); 


        IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards); 
        void AddStandardSubjects(IList<StandardSubject> standardSubjects);
        void EditStandardSubjects(IList<StandardSubject> standardSubjects);
        IList<StandardSubject> GetStandardSubjects(int? classId);

        void DeleteStandardSubjects(IList<int> ids);
        void DeleteClassStandards(IList<ClassStandard> classStandards);
        IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId);

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
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new StandardDataAccess(uow).Insert(standards);
                uow.Commit();
            }
        }
        
        public Standard GetStandardById(int id)
        {
            using (var uow = Read())
            {
                return new StandardDataAccess(uow).GetById(id);
            }
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            using (var uow = Read())
            {
                var res = new StandardDataAccess(uow).GetStandards( new StandardQuery
                    {
                        ClassId = classId,
                        GradeLavelId = gradeLevelId,
                        StandardSubjectId = subjectId,
                        ParentStandardId = parentStandardId,
                        AllStandards = allStandards
                    });
                return PrepareStandards(res);
            }
        }

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            if(standardSubjects != null && standardSubjects.Count > 0)
                using (var uow = Update())
                {
                    new StandardSubjectDataAccess(uow).Insert(standardSubjects);
                    uow.Commit();
                }
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
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                new ClassStandardDataAccess(uow).Insert(classStandards);
                uow.Commit();
                return classStandards;
            }
        }
        public void DeleteStandards(IList<Standard> standards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StandardDataAccess(uow).Delete(standards);
                uow.Commit();
            }
        }


        public void EditStandard(IList<Standard> standards)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StandardDataAccess(uow).Update(standards);
                uow.Commit();
            }
        }


        public void EditStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StandardSubjectDataAccess(uow).Update(standardSubjects);
                uow.Commit();
            }        
        }


        public void DeleteStandardSubjects(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StandardSubjectDataAccess(uow).Delete(ids);
                uow.Commit();
            } 
        }

        public void DeleteClassStandards(IList<ClassStandard> classStandards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new ClassStandardDataAccess(uow).Delete(classStandards);
                uow.Commit();
            } 
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
            return PrepareStandards(standards);
        }

        public IList<Standard> GetStandards(string filter)
        {
            var commonCoreStandards = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService.GetStandards(filter);
            commonCoreStandards = commonCoreStandards.Where(x => x.AcademicBenchmarkId.HasValue).ToList();
            IList<Standard> standards;
            using (var uow = Read())
            {
                var da = new StandardDataAccess(uow);
                standards = da.SearchStandards(filter);
                if (commonCoreStandards.Count > 0)
                   standards = standards.Union(da.GetStandardsByABIds(commonCoreStandards.Select(x => x.AcademicBenchmarkId.Value).ToList())).ToList();
            }
            return PrepareStandards(standards);
        }

        private IList<Standard> PrepareStandards(IList<Standard> standards)
        {
            var ccStandardService = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService;
            foreach (var standard in standards.Where(standard => standard.AcademicBenchmarkId.HasValue))
            {
                standard.CCStandardCode = ccStandardService.GetStandardCodeByABId(standard.AcademicBenchmarkId.Value);
            }
            return standards;
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            using (var uow = Read())
            {
                var res = new AnnouncementStandardDataAccess(uow).GetAnnouncementStandardsByAnnId(announcementId);
                var standards = PrepareStandards(res.Select(x => x.Standard).ToList());
                foreach (var annStandard in res)
                {
                    annStandard.Standard = standards.First(s => s.Id == annStandard.StandardRef);
                }
                return res;
            }
        }
    }
}
