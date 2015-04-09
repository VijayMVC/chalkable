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
  
    public class DemoStandardService : DemoSchoolServiceBase, IStandardService
    {
        public DemoStandardService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void SetupDefaultData()
        {
            var masterLocator = ServiceLocator.ServiceLocatorMaster;
            var ccStandardCategories = masterLocator.CommonCoreStandardService.GetCCStandardCategories();
            ClearStandardsData();
            var subjectId = 1;
            var subjectIdsDic = ccStandardCategories.ToDictionary(x => x.Id, x =>
                {
                    subjectId++;
                    return subjectId;
                });
            InsertDefaultSubjects(subjectIdsDic, ccStandardCategories);
            InsertDefaultStandards(subjectIdsDic);
            InsertDefaultClassStandards();
        }

        private void ClearStandardsData()
        {
           Storage.ClassStandardStorage.Delete(Storage.ClassStandardStorage.GetAll());
           Storage.StandardStorage.Delete(Storage.StandardStorage.GetAll());
           Storage.StandardSubjectStorage.Delete(Storage.StandardSubjectStorage.GetAll());
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
                Storage.StandardSubjectStorage.Add(standardSubjects);
        }

        private void InsertDefaultStandards(IDictionary<Guid, int> subjectIdsDic)
        {
            var masterLocator = ServiceLocator.ServiceLocatorMaster;
            var abToccMapper = masterLocator.CommonCoreStandardService.GetAbToCCMapper();
            var standardId = 0;//GetStandards(null, null, null).Max(x => x.Id);
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
                Storage.StandardStorage.Add(standards);
        }


        private void InsertDefaultClassStandards()
        {
            var standardsIds = Storage.StandardStorage.GetData().Keys.ToList();
            var classStandards = Storage.ClassStandardStorage.GetAll();
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
            Storage.ClassStandardStorage.Add(newClassStandards);
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

            Storage.StandardStorage.Add(standards);
        }

        public void EditStandard(IList<Standard> standards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.StandardStorage.Update(standards);
        }

        private bool IsInvalidStandardModel(Standard standard)
        {
            return standard.LowerGradeLevelRef.HasValue && standard.UpperGradeLevelRef.HasValue
                   && standard.LowerGradeLevelRef > standard.UpperGradeLevelRef;
        }

        public void DeleteStandards(IList<Standard> standards)
        {
            Storage.StandardStorage.Delete(standards);
        }

        public Standard GetStandardById(int id)
        {
            return Storage.StandardStorage.GetById(id);
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true, bool activeOnly = false)
        {
            var res = Storage.StandardStorage.GetStandarts(new StandardQuery
            {
                ClassId = classId,
                GradeLavelId = gradeLevelId,
                StandardSubjectId = subjectId,
                ParentStandardId = parentStandardId,
                ActiveOnly = activeOnly
            });
            return res;
        }

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            if (standardSubjects != null && standardSubjects.Count > 0)
                Storage.StandardSubjectStorage.Add(standardSubjects);
                
        }

        public void EditStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.StandardSubjectStorage.Update(standardSubjects);
        }

        public IList<StandardSubject> GetStandardSubjects(int? classId)
        {
            var res = Storage.StandardSubjectStorage.GetAll();
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
            Storage.StandardSubjectStorage.Delete(ids);
        }

        public void DeleteClassStandards(IList<ClassStandard> classStandards)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassStandardStorage.Delete(classStandards);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            var annStandards = Storage.AnnouncementStandardStorage.GetAll().Where(x => x.AnnouncementRef == announcementId);
            IList<Standard> standards = Storage.StandardStorage.GetAll().Where(x => annStandards.Any(y => y.StandardRef == x.Id)).ToList();
            return annStandards.Select(announcementStandard => 
                new AnnouncementStandardDetails
                {
                    AnnouncementRef = announcementStandard.AnnouncementRef, 
                    Standard = standards.FirstOrDefault(x => x.Id == announcementStandard.StandardRef), 
                    StandardRef = announcementStandard.StandardRef
                }).ToList();
        }

        public IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            return Storage.ClassStandardStorage.Add(classStandards);
        }


        public Standard GetStandardByABId(Guid id)
        {
            return Storage.StandardStorage.GetAll().FirstOrDefault(x => x.AcademicBenchmarkId == id);
        }



        public IList<Standard> GetStandards(IList<int> standardIds)
        {
            if (standardIds == null || standardIds.Count == 0)
                return new List<Standard>();
            var standards = Storage.StandardStorage.GetAll().Where(s=>standardIds.Contains(s.Id)).ToList();
            return standards;
        }

        public IList<Standard> GetStandards(string filter, int? classId, bool activeOnly = false)
        {
            var standards = Storage.StandardStorage.SearchStandards(filter, activeOnly);
            if (classId.HasValue)
            {
                var standardsByClass = GetStandards(classId, null, null, null, false, activeOnly);
                standards = standards.Where(s => standardsByClass.Any(s2 => s2.Id == s.Id)).ToList();
            }
            return standards;
        }

        public StandardTreePath GetStandardParentsSubTree(int standardId)
        {
             return Storage.StandardStorage.GetStandardParentsSubTree(standardId);
        }
    }
}
