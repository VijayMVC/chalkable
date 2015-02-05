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
            return Storage.StandardStorage.GetStandarts(new StandardQuery
            {
                ClassId = classId,
                GradeLavelId = gradeLevelId,
                StandardSubjectId = subjectId,
                ParentStandardId = parentStandardId
            });
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


        public StandardDetailsInfo GetStandardDetailsById(int standardId)
        {
            var standard = GetStandardById(standardId);
            CommonCoreStandard ccStandard = null;
            if (standard.AcademicBenchmarkId.HasValue)
                ccStandard = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService.GetStandardByABId(
                        standard.AcademicBenchmarkId.Value);
            return StandardDetailsInfo.Create(standard, ccStandard);
        }

        public IList<StandardDetailsInfo> GetStandardsDetails(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            var standards = GetStandards(classId, gradeLevelId, subjectId, parentStandardId, allStandards);
            return PrepareStandardsDetailsInfo(standards);
        }


        public IList<StandardDetailsInfo> GetStandardsDetails(IList<int> standardIds)
        {
            if (standardIds == null || standardIds.Count == 0)
                return new List<StandardDetailsInfo>();
            var standards = Storage.StandardStorage.GetAll().Where(s=>standardIds.Contains(s.Id)).ToList();
            return PrepareStandardsDetailsInfo(standards);
        }

        private IList<StandardDetailsInfo> PrepareStandardsDetailsInfo(IList<Standard> standards)
        {
            var abIds = standards.Where(s => s.AcademicBenchmarkId.HasValue).Select(s => s.AcademicBenchmarkId.Value).ToList();
            var ccStandards = ServiceLocator.ServiceLocatorMaster.CommonCoreStandardService.GetStandardsByABIds(abIds);
            var res = new List<StandardDetailsInfo>();
            foreach (var standard in standards)
            {
                CommonCoreStandard ccStandard = null;
                if (standard.AcademicBenchmarkId.HasValue)
                    ccStandard = ccStandards.FirstOrDefault(x => x.AcademicBenchmarkId == standard.AcademicBenchmarkId);
                res.Add(StandardDetailsInfo.Create(standard, ccStandard));
            }
            return res;

        }


        public IList<StandardDetailsInfo> GetStandardsDetails(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
