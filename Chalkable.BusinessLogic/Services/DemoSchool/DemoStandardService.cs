using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStandardStorage : BaseDemoIntStorage<Standard>
    {
        public DemoStandardStorage()
            : base(x => x.Id)
        {
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            var res = GetStandards(new StandardQuery
            {
                ClassId = classId,
                GradeLavelId = gradeLevelId,
                StandardSubjectId = subjectId,
                ParentStandardId = parentStandardId
            });
            return res;
        }


        public IList<Standard> GetStandards(StandardQuery query)
        {
            var standards = data.Select(x => x.Value);
            if (query.StandardSubjectId.HasValue)
                standards = standards.Where(x => x.StandardSubjectRef == query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
                standards =
                    standards.Where(
                        x => query.GradeLavelId <= x.LowerGradeLevelRef && query.GradeLavelId >= x.UpperGradeLevelRef);
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                standards = standards.Where(x => x.ParentStandardRef == query.ParentStandardId);

            if (query.ClassId.HasValue)
            {
                var classStandarts = StorageLocator.ClassStandardStorage.GetAll(query.ClassId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }
            //if (query.CourseId.HasValue)
            //{
            //    var classStandarts = Storage.ClasStandardStorage.GetAll(query.CourseId).Select(x => x.StandardRef);
            //    standards = standards.Where(x => classStandarts.Contains(x.Id));
            //}

            return standards.ToList();
        }


        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            var annStandards = StorageLocator.AnnouncementStandardStorage.GetAll().Where(x => x.AnnouncementRef == announcementId);
            IList<Standard> standards = StorageLocator.StandardStorage.GetAll().Where(x => annStandards.Any(y => y.StandardRef == x.Id)).ToList();
            return annStandards.Select(announcementStandard =>
                new AnnouncementStandardDetails
                {
                    AnnouncementRef = announcementStandard.AnnouncementRef,
                    Standard = standards.FirstOrDefault(x => x.Id == announcementStandard.StandardRef),
                    StandardRef = announcementStandard.StandardRef
                }).ToList();
        }
    }

    public class DemoStandardSubjectStorage : BaseDemoIntStorage<StandardSubject>
    {
        public DemoStandardSubjectStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoClassStandardStorage : BaseDemoIntStorage<ClassStandard>
    {
        public DemoClassStandardStorage()
            : base(null, true)
        {
        }

        public new void Delete(IList<ClassStandard> classStandards)
        {
            foreach (var item in classStandards.Select(classStandard => data.First(
                x =>
                    x.Value.ClassRef == classStandard.ClassRef &&
                    x.Value.StandardRef == classStandard.StandardRef)))
            {
                Delete(item.Key);
            }
        }

        public IList<ClassStandard> GetAll(int? classId)
        {
            var items = data.Select(x => x.Value);
            if (classId.HasValue)
                items = items.Where(x => x.ClassRef == classId);
            return items.ToList();
        }
    }

    public class DemoStandardService : DemoSchoolServiceBase, IStandardService
    {
        private DemoStandardSubjectStorage StandardSubjectStorage { get; set; }
        private DemoStandardStorage StandardStorage { get; set; }
        private DemoClassStandardStorage ClassStandardStorage { get; set; }
        public DemoStandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StandardSubjectStorage = new DemoStandardSubjectStorage();
            StandardStorage = new DemoStandardStorage();
            ClassStandardStorage = new DemoClassStandardStorage();
        }

        public void SetupDefaultData()
        {
            var masterLocator = ServiceLocator.ServiceLocatorMaster;
            var ccStandardCategories = masterLocator.CommonCoreStandardService.GetCCStandardCategories();
            var subjectId = GetStandardSubjects(null).Max(x => x.Id);
            var subjectIdsDic = ccStandardCategories.ToDictionary(x => x.Id, x =>
                {
                    subjectId++;
                    return subjectId;
                });
            InsertDefaultSubjects(subjectIdsDic, ccStandardCategories);
            InsertDefaultStandards(subjectIdsDic);
            InsertDefaultClassStandards();
        }

        private void InsertDefaultSubjects(IDictionary<Guid, int> subjectIdsDic
            , IEnumerable<CommonCoreStandardCategory> ccStandardCategories)
        {
            var standardSubjects = ccStandardCategories.Select(standardCategory => new StandardSubject
                {
                    Id = subjectIdsDic[standardCategory.Id], 
                    Name = standardCategory.Name, 
                    IsActive = true
                }).ToList();
            if (standardSubjects.Count > 0)
                StandardSubjectStorage.Add(standardSubjects);
        }

        private void InsertDefaultStandards(IDictionary<Guid, int> subjectIdsDic)
        {
            var masterLocator = ServiceLocator.ServiceLocatorMaster;
            var abToccMapper = masterLocator.CommonCoreStandardService.GetAbToCCMapper();
            var standardId = GetStandards(null, null, null).Max(x => x.Id);
            IDictionary<Guid, int> ccStandardsIdsDic = new Dictionary<Guid, int>();
            var ccStandards = new List<CommonCoreStandard>();
            foreach (var ccStandard in abToccMapper.Values)
            {
                if (!ccStandardsIdsDic.ContainsKey(ccStandard.Id))
                {
                    standardId++;
                    ccStandardsIdsDic.Add(ccStandard.Id, standardId);
                    ccStandards.Add(ccStandard);
                }
            }
            var standards = new List<Standard>();
            foreach (var ccStandard in ccStandards)
            {
                if (ccStandard.AcademicBenchmarkId.HasValue && subjectIdsDic.ContainsKey(ccStandard.StandardCategoryRef)
                    && (!ccStandard.ParentStandardRef.HasValue || ccStandardsIdsDic.ContainsKey(ccStandard.ParentStandardRef.Value)))
                    standards.Add(new Standard
                        {
                            Id = ccStandardsIdsDic[ccStandard.Id],
                            AcademicBenchmarkId = ccStandard.AcademicBenchmarkId,
                            CCStandardCode = ccStandard.Code,
                            Description = ccStandard.Description,
                            IsActive = true,
                            Name = ccStandard.Code,
                            ParentStandardRef = ccStandard.ParentStandardRef.HasValue ? ccStandardsIdsDic[ccStandard.ParentStandardRef.Value] : (int?)null,
                            StandardSubjectRef = subjectIdsDic[ccStandard.StandardCategoryRef]
                        });
            }
            if (standards.Count > 0)
                StandardStorage.Add(standards);
        }


        private void InsertDefaultClassStandards()
        {
            var standardsIds = StandardStorage.GetData().Keys.ToList();
            var classStandards = ClassStandardStorage.GetAll();
            standardsIds = standardsIds.Where(x => classStandards.All(y => y.StandardRef != x)).ToList();
            var classes =  ServiceLocator.ClassService.GetAll();
            var newClassStandards = new List<ClassStandard>();
            foreach (var c in classes)
            {
                newClassStandards.AddRange(standardsIds.Select(id=> new ClassStandard
                {
                    ClassRef = c.Id,
                    StandardRef = id
                }).ToList());
            }
            ClassStandardStorage.Add(newClassStandards);
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

            if (standards.Any(IsInvalidStandardModel))
                throw new ChalkableException("Invalid params. LowerGradeLevelId can not be greater than upperGradeLevelId");

            StandardStorage.Add(standards);
        }

        public void EditStandard(IList<Standard> standards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            StandardStorage.Update(standards);
        }

        private bool IsInvalidStandardModel(Standard standard)
        {
            return standard.LowerGradeLevelRef.HasValue && standard.UpperGradeLevelRef.HasValue
                   && standard.LowerGradeLevelRef > standard.UpperGradeLevelRef;
        }

        public void DeleteStandards(IList<Standard> standards)
        {
            StandardStorage.Delete(standards);
        }

        public Standard GetStandardById(int id)
        {
            return StandardStorage.GetById(id);
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            return StandardStorage.GetStandards(classId, gradeLevelId, subjectId, parentStandardId,
                allStandards);
        }

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            if (standardSubjects != null && standardSubjects.Count > 0)
                StandardSubjectStorage.Add(standardSubjects);
                
        }

        public void EditStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            StandardSubjectStorage.Update(standardSubjects);
        }

        public IList<StandardSubject> GetStandardSubjects(int? classId)
        {
            var res = StandardSubjectStorage.GetAll();
            if (classId.HasValue)
            {
                var standards = GetStandards(classId, null, null);
                res = res.Where(x => standards.Any(s => s.StandardSubjectRef == x.Id)).ToList();
            }
            return res;
        }
        
        public void DeleteStandardSubjects(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            StandardSubjectStorage.Delete(ids);
        }

        public void DeleteClassStandards(IList<ClassStandard> classStandards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ClassStandardStorage.Delete(classStandards);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            return StandardStorage.GetAnnouncementStandards(announcementId);
        }

        public IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            return ClassStandardStorage.Add(classStandards);
        }

        public Standard GetStandardByABId(Guid id)
        {
            return StandardStorage.GetAll().FirstOrDefault(x => x.AcademicBenchmarkId == id);
        }

        public IList<Standard> GetStandards(IList<int> standardIds)
        {
            if (standardIds == null || standardIds.Count == 0)
                return new List<Standard>();
            var standards = StandardStorage.GetAll().Where(s=>standardIds.Contains(s.Id)).ToList();
            return standards;
        }

        public IList<Standard> GetStandards(string filter)
        {
            throw new NotImplementedException();
        }

    }
}
