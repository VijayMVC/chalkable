using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public abstract class BaseFeedReportHandler : IReportHandler<FeedReportInputModel>
    {
        protected class BaseFeedReportInfo
        {
            public IList<ClassDetails> Classes { get; set; }
            public IList<Staff> Staffs { get; set; }
            public IList<DayType> DayTypes { get; set; }
            public Person Person { get; set; }
            public string SchoolName { get; set; }
            public string SchoolYearName { get; set; }
        }

        protected BaseFeedReportInfo PrepareBaseReportData(FeedReportInputModel inputModel, IServiceLocatorSchool serviceLocator)
        {
            var context = serviceLocator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            Trace.Assert(context.PersonId.HasValue);
            Trace.Assert(context.SchoolLocalId.HasValue);

            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();

            var res = new BaseFeedReportInfo();

            res.Person = serviceLocator.PersonService.GetPerson(context.PersonId.Value);
            res.SchoolYearName = sy.Name;
            res.SchoolName = serviceLocator.SchoolService.GetSchool(context.SchoolLocalId.Value).Name;

            var isStudent = context.Role == CoreRoles.STUDENT_ROLE;
            var teacherId = isStudent ? null : (int?)res.Person.Id;
            var studentId = isStudent ? (int?)res.Person.Id : null;

            res.DayTypes = serviceLocator.DayTypeService.GetDayTypes();

            res.Classes = inputModel.ClassId.HasValue
                ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(inputModel.ClassId.Value) }
                : serviceLocator.ClassService.GetClasses(sy.Id, studentId, teacherId);

            var classTeachers = res.Classes.SelectMany(x => x.ClassTeachers.Select(y => y)).ToList();
            var staffIds = classTeachers.Select(x => x.PersonRef).Distinct().ToList();

            res.Staffs = staffIds.Select(y => serviceLocator.StaffService.GetStaff(y)).ToList();
            return res;
        }

        public abstract object PrepareDataSource(FeedReportInputModel settings, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator);
        public abstract string ReportDefinitionFile { get; }

    }

}
