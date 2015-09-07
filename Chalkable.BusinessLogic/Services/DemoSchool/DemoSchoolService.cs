﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Chlk;
using Chalkable.Data.School.Model.Sis;
using ISchoolService = Chalkable.BusinessLogic.Services.School.ISchoolService;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolStorage : BaseDemoIntStorage<Data.School.Model.Sis.School>
    {
        public DemoSchoolStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoSchoolOptionStorage : BaseDemoIntStorage<SchoolOption>
    {
        public DemoSchoolOptionStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoSchoolService : DemoSchoolServiceBase, ISchoolService
    {
        private DemoSchoolStorage SchoolStorage { get; set; }
        private DemoSchoolOptionStorage SchoolOptionStorage { get; set; }
        public DemoSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            SchoolStorage = new DemoSchoolStorage();
            SchoolOptionStorage = new DemoSchoolOptionStorage();
        }

        public void Add(Data.School.Model.Sis.School school)
        {
            SchoolStorage.Add(school);

            var l = new List<SchoolInfo>
                {
                    new SchoolInfo
                        {
                            Name = school.Name,
                            IsChalkableEnabled = school.IsChalkableEnabled,
                            LocalId =  school.Id
                        }
                };
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(l, Context.DistrictId.Value);
        }

        public void Add(IList<Data.School.Model.Sis.School> schools)
        {
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            
            SchoolStorage.Add(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Edit(IList<Data.School.Model.Sis.School> schools)
        {
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            SchoolStorage.Update(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Edit(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Delete(IList<Data.School.Model.Sis.School> schools)
        {
            SchoolStorage.Delete(schools);
        }

        public IList<Data.School.Model.Sis.School> GetSchools()
        {
            return SchoolStorage.GetAll();
        }

        public void AddSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            SchoolOptionStorage.Add(schoolOptions);
        }

        public void EditSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            SchoolOptionStorage.Update(schoolOptions);
        }

        public void DeleteSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            SchoolOptionStorage.Delete(schoolOptions);
        }

        public SchoolOption GetSchoolOption()
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException(); 
            return SchoolOptionStorage.GetById(Context.SchoolLocalId.Value);
        }

        public StartupData GetStartupData()
        {
            var mps = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(Context.SchoolYearId);
            var markingPeriod = mps.Where(x=>x.StartDate <= Context.NowSchoolYearTime).OrderBy(x=>x.StartDate).LastOrDefault();

            var startupData = new StartupData
            {
                UnshownNotificationsCount = ServiceLocator.NotificationService.GetUnshownNotifications().Count,
                AttendanceReasons = ServiceLocator.AttendanceReasonService.List(),
                AlternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores(),
                MarkingPeriods = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(Context.SchoolYearId),
                AlphaGrades = ServiceLocator.AlphaGradeService.GetAlphaGrades(),
                GradingComments = ServiceLocator.GradingCommentService.GetGradingComments(),
                SchoolOption = GetSchoolOption(),
                Person = ServiceLocator.PersonService.GetPersonDetails(Context.PersonId.Value),
                Classes = ((DemoClassService)ServiceLocator.ClassService).GetClassesSortedByPeriod(),
                GradingPeriod = markingPeriod != null && Context.SchoolLocalId.HasValue
                    ? ServiceLocator.GradingPeriodService.GetGradingPeriodDetails(markingPeriod.SchoolYearRef,
                        Context.NowSchoolYearTime.Date)
                    : null,
            };

            var alphaGradesForClasses = new Dictionary<int, IList<AlphaGrade>>();
            var alphaGradesForClassStandards = new Dictionary<int, IList<AlphaGrade>>();

            foreach (var classDetail in startupData.Classes)
            {
                alphaGradesForClasses.Add(classDetail.Id, ServiceLocator.AlphaGradeService.GetAlphaGrades());
                alphaGradesForClassStandards.Add(classDetail.Id, ServiceLocator.AlphaGradeService.GetAlphaGrades());
            }
            startupData.AlphaGradesForClassStandards = alphaGradesForClassStandards;
            startupData.AlphaGradesForClasses = alphaGradesForClasses;

            return startupData;
        }
    }
}