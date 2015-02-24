﻿using System;
using System.Collections.Generic;
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

        public void DeleteStandard(int id)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.StandardStorage.Delete(id);
        }

        public void DeleteStandards(IList<int> ids)
        {
            Storage.StandardStorage.Delete(ids);
        }

        public Standard GetStandardById(int id)
        {
            return Storage.StandardStorage.GetById(id);
        }

        public IList<Standard> GetStandards(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            var res = Storage.StandardStorage.GetStandarts(new StandardQuery
            {
                ClassId = classId,
                GradeLavelId = gradeLevelId,
                StandardSubjectId = subjectId,
                ParentStandardId = parentStandardId
            });
            return PrepareStandardsDetailsInfo(res);
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

            Storage.ClasStandardStorage.Delete(classStandards);
        }

        public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        {
            var annStandards = Storage.AnnouncementStandardStorage.GetAll().Where(x => x.AnnouncementRef == announcementId);
            IList<Standard> standards = Storage.StandardStorage.GetAll().Where(x => annStandards.Any(y => y.StandardRef == x.Id)).ToList();
            standards = PrepareStandardsDetailsInfo(standards);
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
            return Storage.ClasStandardStorage.Add(classStandards);
        }


        public Standard GetStandardByABId(Guid id)
        {
            return Storage.StandardStorage.GetAll().First(x => x.AcademicBenchmarkId == id);
        }



        public IList<Standard> GetStandards(IList<int> standardIds)
        {
            if (standardIds == null || standardIds.Count == 0)
                return new List<Standard>();
            var standards = Storage.StandardStorage.GetAll().Where(s=>standardIds.Contains(s.Id)).ToList();
            return PrepareStandardsDetailsInfo(standards);
        }

        private IList<Standard> PrepareStandardsDetailsInfo(IList<Standard> standards)
        {
            var abIds = standards.Where(s => s.AcademicBenchmarkId.HasValue).Select(s => s.AcademicBenchmarkId.Value).ToList();
            var ccDisc = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService.GetStandardsCodeByABIds(abIds);
            foreach (var standard in standards)
            {
                if (standard.AcademicBenchmarkId.HasValue && ccDisc.ContainsKey(standard.AcademicBenchmarkId.Value))
                    standard.CCStandardCode = ccDisc[standard.AcademicBenchmarkId.Value];
            }
            return standards;
        }
        

        public IList<Standard> GetStandards(string filter)
        {
            throw new NotImplementedException();
        }

    }
}
