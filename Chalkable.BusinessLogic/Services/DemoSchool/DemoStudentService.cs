﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentService : DemoSchoolServiceBase, IStudentService
    {
        public DemoStudentService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void EditStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public StudentDetails GetById(int id, int schoolYearId)
        {
            return Storage.StudentStorage.GetStudentDeatils(id, schoolYearId);
        }

        public IList<StudentDetails> GetClassStudents(int classId, int markingPeriodId, bool? isEnrolled = null)
        {
            return Storage.StudentStorage.GetClassStudents(classId, markingPeriodId, isEnrolled);
        }

        public PaginatedList<StudentDetails> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName,
            int start, int count)
        {
            return Storage.StudentStorage.SearStudents(schoolYearId, classId, teacherId, filter, orderByFirstName, start, count);
        }
        
        public IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            return Storage.StudentStorage.GetTeacherStudents(teacherId, schoolYearId);
        }

        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            return Storage.StudentHealthConditionStorage.GetByStudentId(studentId);
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId)
        {
            var classRank = new ClassRank
            {
                StudentId = studentId,
                ClassSize = 10
            };

            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;

            var discipline = Storage.StiDisciplineStorage.GetList(DateTime.Today);

            var infractions = new List<StiConnector.Connectors.Model.Infraction>();

            foreach (var disciplineReferral in discipline)
            {
                infractions.AddRange(disciplineReferral.Infractions);
            }

            var chlkInfractions = ServiceLocator.InfractionService.GetInfractions();

            var infractionSummaries = from infr in infractions
                                      group infr by infr.Id
                                          into g
                                          select new { Id = g.Key, Count = g.Count() };

            var attendances = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(studentId,
                DateTime.Today.AddDays(-1), DateTime.Today).Select(sectionAttendanceSummary => new SectionAbsenceSummary
                {
                    SectionId = sectionAttendanceSummary.SectionId,
                    Tardies = sectionAttendanceSummary.Students.Select(x => x.Tardies).Sum(),
                    Absences = sectionAttendanceSummary.Students.Select(x => x.Absences).Sum(),

                }).ToList();

            var infractionSummary = infractionSummaries.Select(x => new InfractionSummary
            {
                StudentId = studentId,
                InfractionId = x.Id,
                Occurrences = x.Count
            }).ToList();

            var nowDashboard = new NowDashboard
            {
                ClassRank = classRank,
                Infractions = infractionSummary,
                //SectionAttendance = attendances,
                Scores = Storage.StiActivityScoreStorage.GetScores(studentId)
            };
            var student = ServiceLocator.StudentService.GetById(studentId, syId);
            var activitiesids = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();
            var anns = Storage.AnnouncementStorage.GetAnnouncements(new AnnouncementsQuery { SisActivitiesIds = activitiesids }).Announcements;
            var res = StudentSummaryInfo.Create(student, nowDashboard, chlkInfractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }


        public StudentExplorerInfo GetStudentExplorerInfo(int studentId, int schoolYearId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            //var date = Context.NowSchoolYearTime;
            var student = GetById(studentId, schoolYearId);
            var classes = ServiceLocator.ClassService.GetStudentClasses(schoolYearId, studentId).ToList();
            var classPersons = ServiceLocator.ClassService.GetClassPersons(studentId, true);
            classes = classes.Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            var inowStExpolorer = new StudentExplorerDashboard
                {
                    Averages = new List<StudentAverage>(),
                    Standards = new Stack<StandardScore>(),
                    Activities = new List<Activity>()
                };
            var standards = ServiceLocator.StandardService.GetStandards(null, null, null);
            IList<int> importanActivitiesIds = new List<int>();
            IList<AnnouncementComplex> announcements = new List<AnnouncementComplex>();
            if (inowStExpolorer.Activities.Any())
            {
                foreach (var classDetailse in classes)
                {
                    var activity = inowStExpolorer.Activities.Where(x => x.SectionId == classDetailse.Id)
                                                .OrderByDescending(x => x.MaxScore * x.WeightMultiplier + x.WeightAddition).FirstOrDefault();
                    if (activity == null) continue;
                    importanActivitiesIds.Add(activity.Id);
                }
                announcements = DoRead(uow => new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId.Value).GetByActivitiesIds(importanActivitiesIds));
            }
            return StudentExplorerInfo.Create(student, classes, inowStExpolorer.Averages.ToList()
                , inowStExpolorer.Standards.ToList(), announcements, standards);
        }
    }
}
