﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentService
    {
        void AddStudents(IList<Student> students);
        void EditStudents(IList<Student> students);
        void DeleteStudents(IList<Student> students);

        void AddStudentSchools(IList<StudentSchool> studentSchools);
        void EditStudentSchools(IList<StudentSchool> studentSchools);
        void DeleteStudentSchools(IList<StudentSchool> studentSchools);

        IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId);
        IList<StudentDetails> GetClassStudents(int classId, int markingPeriodId, bool? isEnrolled = null);
        PaginatedList<StudentDetails> SearchStudents(int schoolYearId, int? classId, int? teacherId, string filter, bool orderByFirstName, int start, int count);

        StudentDetails GetById(int id, int schoolYearId);
        IList<StudentHealthCondition> GetStudentHealthConditions(int studentId);
        StudentSummaryInfo GetStudentSummaryInfo(int studentId);
    }

    public class StudentService : SisConnectedService, IStudentService
    {
        public StudentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Insert(students));
        }

        public void EditStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Update(students));
        }

        public void DeleteStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Delete(students));
        }
        private void ModifyStudet(Action<StudentDataAccess> action)
        {
            Modify(uow => action(new StudentDataAccess(uow)));
        }
        private void ModifyStudentSchool(Action<StudentSchoolDataAccess> action)
        {
            Modify(uow => action(new StudentSchoolDataAccess(uow, Context.SchoolLocalId)));
        }
        private void Modify(Action<UnitOfWork> modifyAction)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                modifyAction(uow);
                uow.Commit();
            }
        }
        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Insert(studentSchools));
        }
        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Update(studentSchools));
        }
        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Delete(studentSchools));
        }

        public StudentDetails GetById(int id, int schoolYearId)
        {
            using (var uow = Read())
            {
                var da = new StudentDataAccess(uow);
                return da.GetDetailsById(id, schoolYearId);
            }
        }

        public IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            return DoRead(u => new StudentDataAccess(u).GetTeacherStudents(teacherId, schoolYearId));
        }

        public IList<StudentDetails> GetClassStudents(int classId, int markingPeriodId, bool? isEnrolled = null)
        {
            return DoRead(u => new StudentDataAccess(u).GetStudents(classId, markingPeriodId, isEnrolled));
        }

        public PaginatedList<StudentDetails> SearchStudents(int schoolYearId, int? classId, int? teacherId, string filter, bool orderByFirstName, int start, int count)
        {
            return DoRead( u => new StudentDataAccess(u).SearchStudents(schoolYearId, classId, teacherId, filter,
                            orderByFirstName, start, count));
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId)
        {
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var nowDashboard = ConnectorLocator.StudentConnector.GetStudentNowDashboard(syId, studentId);
            var student = ServiceLocator.PersonService.GetPerson(studentId);
            var infractions = ServiceLocator.InfractionService.GetInfractions();
            var activitiesIds = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();
            IList<AnnouncementComplex> anns;
            using (var uow = Read())
            {
                var da = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId);
                anns = da.GetByActivitiesIds(activitiesIds);
            }
            var res = StudentSummaryInfo.Create(student, nowDashboard, infractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }

        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            if (CanGetHealthConditions())
            {
                var healthConditions = ConnectorLocator.StudentConnector.GetStudentConditions(studentId);

                if (healthConditions == null)
                    return new List<StudentHealthCondition>();

                var result = (from studentCondition in healthConditions
                              where studentCondition != null
                              select new StudentHealthCondition
                              {
                                  Id = studentCondition.Id,
                                  Name = studentCondition.Name,
                                  Description = studentCondition.Description,
                                  IsAlert = studentCondition.IsAlert,
                                  MedicationType = studentCondition.MedicationType,
                                  Treatment = studentCondition.Treatment
                              }).ToList();
                return result;
            }
            return new List<StudentHealthCondition>();
        }

        private bool CanGetHealthConditions()
        {
            return Context.Role != CoreRoles.STUDENT_ROLE
                   && (ClaimInfo.HasPermission(Context.Claims, new List<string> { ClaimInfo.VIEW_HEALTH_CONDITION })
                       || ClaimInfo.HasPermission(Context.Claims, new List<string> { ClaimInfo.VIEW_MEDICAL }));
        }
    }
}
