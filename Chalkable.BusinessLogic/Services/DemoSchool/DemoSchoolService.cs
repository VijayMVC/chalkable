using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using ISchoolService = Chalkable.BusinessLogic.Services.School.ISchoolService;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoSchoolService : DemoSchoolServiceBase, ISchoolService
    {
        public DemoSchoolService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            Storage.SchoolStorage.Add(school);

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

        public void Add(IList<Data.School.Model.School> schools)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            
            Storage.SchoolStorage.Add(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Add(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Edit(IList<Data.School.Model.School> schools)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            Storage.SchoolStorage.Update(schools);
            ServiceLocator.ServiceLocatorMaster.SchoolService.Edit(schools.Select(x => new SchoolInfo
            {
                LocalId = x.Id,
                Name = x.Name
            }).ToList(), Context.DistrictId.Value);
        }

        public void Delete(IList<Data.School.Model.School> schools)
        {
            Storage.SchoolStorage.Delete(schools);
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            return Storage.SchoolStorage.GetAll();
        }

        public void AddSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            Storage.SchoolOptionStorage.Add(schoolOptions);
        }

        public void EditSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            Storage.SchoolOptionStorage.Update(schoolOptions);
        }

        public void DeleteSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            Storage.SchoolOptionStorage.Delete(schoolOptions);
        }

        public SchoolOption GetSchoolOption()
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException(); 
            return Storage.SchoolOptionStorage.GetById(Context.SchoolLocalId.Value);
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
                AlphaGrades = Storage.AlphaGradeStorage.GetAll(),
                GradingComments = ServiceLocator.GradingCommentService.GetGradingComments(),
                SchoolOption = GetSchoolOption(),
                Person = ServiceLocator.PersonService.GetPersonDetails(Context.PersonId.Value),
                Classes = Storage.ClassStorage.GetClassesSortedByPeriod(),
                GradingPeriod = markingPeriod != null && Context.SchoolLocalId.HasValue
                    ? ServiceLocator.GradingPeriodService.GetGradingPeriodDetails(markingPeriod.SchoolYearRef,
                        Context.NowSchoolYearTime.Date)
                    : null,
            };

            var alphaGradesForClasses = new Dictionary<int, IList<AlphaGrade>>();
            var alphaGradesForClassStandards = new Dictionary<int, IList<AlphaGrade>>();

            foreach (var classDetail in startupData.Classes)
            {
                alphaGradesForClasses.Add(classDetail.Id, Storage.AlphaGradeStorage.GetForClass(classDetail.Id));
                alphaGradesForClassStandards.Add(classDetail.Id, Storage.AlphaGradeStorage.GetForClassStandarts(classDetail.Id));
            }
            startupData.AlphaGradesForClassStandards = alphaGradesForClassStandards;
            startupData.AlphaGradesForClasses = alphaGradesForClasses;

            return startupData;
        }
    }
}