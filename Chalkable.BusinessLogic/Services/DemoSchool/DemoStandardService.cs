using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementStandardStorage : BaseDemoIntStorage<AnnouncementStandard>
    {
        public DemoAnnouncementStandardStorage()
            : base(null, true)
        {
        }

        public void Delete(int announcementId, int standardId)
        {
            var annStandarts =
                data.Where(x => x.Value.AnnouncementRef == announcementId && x.Value.StandardRef == standardId)
                    .Select(x => x.Key)
                    .ToList();
            Delete(annStandarts);
        }

        public void DeleteForStandard(int standardId)
        {
            var annStandarts =
                data.Where(x => x.Value.StandardRef == standardId)
                    .Select(x => x.Key)
                    .ToList();
            Delete(annStandarts);
        }

        public IList<AnnouncementStandard> GetAll(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }
    }

    public class DemoStandardStorage : BaseDemoIntStorage<Standard>
    {
        public DemoStandardStorage()
            : base(x => x.Id)
        {
        }

        public IList<Standard> SearchStandards(string filter, bool activeOnly = false)
        {
            if (string.IsNullOrEmpty(filter)) return new List<Standard>();
            var words = filter.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return new List<Standard>();

            var standards = (activeOnly ? data.Where(x => x.Value.IsActive) : data).Select(x => x.Value).ToList();
            var res = (new List<Standard>()).AsEnumerable();
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                res = res.Union(standards.Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                           || (!string.IsNullOrEmpty(s.Description) && s.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                ));
            }
            return res.ToList();
        }

        public StandardTreePath GetStandardParentsSubTree(int standardId)
        {
            int? currentParentId = standardId;
            Standard currentStandard;
            var allStandards = new List<Standard>();
            IList<Standard> path = new List<Standard>();
            var standards = GetData().Select(x => x.Value).ToList();
            var lastChild = standards.Where(x => x.ParentStandardRef == currentParentId && x.IsActive).ToList();
            allStandards.AddRange(lastChild);
            var res = new StandardTreePath
            {
                AllStandards = new List<Standard>(),
                Path = new List<Standard>()
            };
            while (currentParentId.HasValue)
            {
                currentStandard = GetById(currentParentId.Value);
                if (!currentStandard.IsActive)
                    return res;

                currentParentId = currentStandard.ParentStandardRef;
                allStandards.AddRange(currentParentId.HasValue
                                          ? standards.Where(x => x.ParentStandardRef == currentParentId && x.IsActive).ToList()
                                          : standards.Where(x => !x.ParentStandardRef.HasValue && x.StandardSubjectRef == currentStandard.StandardSubjectRef && x.IsActive).ToList());
                path.Add(currentStandard);
            }
            res.Path = path.Reverse().ToList();
            res.AllStandards = allStandards;
            return res;
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
        private DemoAnnouncementStandardStorage AnnouncementStandardStorage { get; set; }
        public DemoStandardService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            StandardSubjectStorage = new DemoStandardSubjectStorage();
            StandardStorage = new DemoStandardStorage();
            ClassStandardStorage = new DemoClassStandardStorage();
            AnnouncementStandardStorage = new DemoAnnouncementStandardStorage();
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true, bool activeOnly = false)
        {
            var res = GetStandards(new StandardQuery
            {
                ClassId = classId,
                GradeLavelId = gradeLevelId,
                StandardSubjectId = subjectId,
                ParentStandardId = parentStandardId,
                ActiveOnly = activeOnly
            });
            return res;
        }

        public IList<Standard> GetStandards(StandardQuery query)
        {
            var standards = StandardStorage.GetData().Select(x => x.Value);
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
                var classStandarts = ClassStandardStorage.GetAll(query.ClassId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }
            //if (query.CourseId.HasValue)
            //{
            //    var classStandarts = Storage.ClasStandardStorage.GetAll(query.CourseId).Select(x => x.StandardRef);
            //    standards = standards.Where(x => classStandarts.Contains(x.Id));
            //}

            return standards.ToList();
        }

        public void SetupDefaultData()
        {
            //TODO: IMPLEMENT LATER

            //var masterLocator = ServiceLocator.ServiceLocatorMaster;
            //var ccStandardCategories = masterLocator.CommonCoreStandardService.GetCCStandardCategories();
            //ClearStandardsData();
            //var subjectId = 1;
            //var subjectIdsDic = ccStandardCategories.ToDictionary(x => x.Id, x =>
              //  {
                //    subjectId++;
                  //  return subjectId;
                //});
            //InsertDefaultSubjects(subjectIdsDic, ccStandardCategories);
            //InsertDefaultStandards(subjectIdsDic);
            //InsertDefaultClassStandards();
        }

        private void ClearStandardsData()
        {
           ClassStandardStorage.Delete(ClassStandardStorage.GetAll());
           StandardStorage.Delete(StandardStorage.GetAll());
           StandardSubjectStorage.Delete(StandardSubjectStorage.GetAll());
        }

        private void InsertDefaultSubjects(IDictionary<Guid, int> subjectIdsDic
            , IEnumerable<Guid> ccStandardCategories)
        {
            throw new NotImplementedException();
        }

        private void InsertDefaultStandards(IDictionary<Guid, int> subjectIdsDic)
        {
            throw new NotImplementedException();
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
            if (standards.Any(IsInvalidStandardModel))
                throw new ChalkableException("Invalid params. LowerGradeLevelId can not be greater than upperGradeLevelId");
            StandardStorage.Add(standards);
        }

        public void AddAnnouncementStandard(AnnouncementStandard standard)
        {
            AnnouncementStandardStorage.Add(standard);
        }

        public void EditStandard(IList<Standard> standards)
        {
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

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if (standardSubjects != null && standardSubjects.Count > 0)
                StandardSubjectStorage.Add(standardSubjects);  
        }

        public void EditStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            StandardSubjectStorage.Update(standardSubjects);
        }

        public IList<StandardSubject> GetStandardSubjects(int? classId)
        {
            var res = StandardSubjectStorage.GetAll();
            if (classId.HasValue)
            {
                var standards = GetStandards(classId, null, null, null);
                res = res.Where(x => standards.Any(s => s.StandardSubjectRef == x.Id)).ToList();
            }
            return res;
        }

        public void DeleteStandardSubjects(IList<int> ids)
        {
            StandardSubjectStorage.Delete(ids);
        }

        public void DeleteClassStandards(IList<ClassStandard> classStandards)
        {
            ClassStandardStorage.Delete(classStandards);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            var annStandards = AnnouncementStandardStorage.GetAll().Where(x => x.AnnouncementRef == announcementId);
            return annStandards.Select(announcementStandard =>
                new AnnouncementStandardDetails
                {
                    AnnouncementRef = announcementStandard.AnnouncementRef,
                    Standard = StandardStorage.GetById(announcementStandard.StandardRef),
                    StandardRef = announcementStandard.StandardRef
                }).ToList();
        }

        public IList<Standard> PrepareStandardsCodesData(IList<Standard> standards)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementStandardDetails> PrepareAnnouncementStandardsCodes(IList<AnnouncementStandardDetails> announcementStandards)
        {
            throw new NotImplementedException();
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

        public IList<Standard> GetStandards(string filter, int? classId, bool activeOnly = false, bool? deepest = null)
        {
            var standards = StandardStorage.SearchStandards(filter, activeOnly);
            if (classId.HasValue)
            {
                var standardsByClass = GetStandards(classId, null, null, null, false, activeOnly);
                standards = standards.Where(s => standardsByClass.Any(s2 => s2.Id == s.Id)).ToList();
            }
            return standards;
        }

        public IList<ClassStandard> GetClassStandards(int standardId)
        {
            return ClassStandardStorage.GetAll().Where(x => x.StandardRef == standardId).ToList();
        }

        public void DeleteAnnouncementStandards(int announcementId, int standardId)
        {
            AnnouncementStandardStorage.Delete(announcementId, standardId);
        }


        public void DeleteAnnouncementStandards(int standardId)
        {
            AnnouncementStandardStorage.Delete(standardId);
        }

        public void DeleteAnnouncementStandards(IList<AnnouncementStandard> standards)
        {
            AnnouncementStandardStorage.Delete(standards);
        }

        public bool ClassStandardsExist(ClassDetails cls)
        {
            return ClassStandardStorage.GetAll().Count(x => x.ClassRef == cls.Id || x.ClassRef == cls.CourseRef) > 0;
        }

        //todo: add preparestadnardsCodeData
        public IList<StandardTreeItem> GetStandardParentsSubTree(int standardId, int? classId)
        {
            return DoRead(uow => new StandardDataAccess(uow).GetStandardParentsSubTree(standardId, classId));
        }

        public void CopyStandardsToAnnouncement(int fromAnnouncementId, int toAnnouncementId, int announcementType)
        {
            throw new NotImplementedException();
        }

        public IList<Standard> GetGridStandardsByPacing(int? classId, int? gradeLevelId, int? subjectId, int? gradingPeriodId,
            int? parentStandardId = null, bool allStandards = true, bool activeOnly = false)
        {
            throw new NotImplementedException();
        }

        public void PrepareToDelete(List<Standard> toDelete)
        {
            throw new NotImplementedException();
        }
    }
}
