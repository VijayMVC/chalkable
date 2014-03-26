using System;
using System.Linq;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStatisticService
    {
        IList<StudentGradeAvgPerMPC> GetStudentsGradePerMPC(int teacherId, IList<int> markingPeriodIds);
        IList<StudentGradeAvgPerClass> GetStudentsGradePerClass(int teacherId, int schoolYearId);
        IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(int classId, int schoolYearId, List<int> markingPeriodIds, int? teacherId, int? studentId = null);
        IList<StudentGradeAvgPerDate> GetStudentGradePerDate(int studentId, int markingPeriodId, int? classId);
        IList<StudentClassGradeStats> GetStudentClassGradeStats(int markingPeriodId, int classId, int? studentId);
        IList<DepartmentGradeAvg> GetDepartmentGradeAvgPerMp(int markingPeriodId, IList<int> gradeLevelIds);
        IList<ClassPersonGradingStats> GetFullGradingStats(int markingPeriodId, int studentId);
        IList<StudentGradingRank> GetStudentGradingRanks(int schoolYearId, int? studentId, int? gradeLevelId, int? classId);
        IList<ChalkableGradeBook> GetGradeBooks(int classId);
        ChalkableGradeBook GetGradeBook(int classId, int gradingPeriodId);
    }
    public class GradingStatisticService : SisConnectedService, IGradingStatisticService
    {
        public GradingStatisticService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        ////TODO: needs test 
        //private GradingStatisticQuery QueryFiltering(GradingStatisticQuery query)
        //{
        //    query.CallerId = Context.UserId;
        //    query.Role = Context.Role.Id;
        //    query.Date = Context.NowSchoolTime.Date;
        //    return query;
        //}

        //public IList<StudentGradeAvgPerMPC> GetStudentsGradePerMPC(Guid teacherId, IList<Guid> markingPeriodIds)
        //{
        //    using (var uow = Read())
        //    {
        //        var query = QueryFiltering(new GradingStatisticQuery
        //            {
        //                TeacherId = teacherId,
        //                MarkingPeriodIds = markingPeriodIds
        //            });
        //        return new GradingStatisticDataAccess(uow).CalcStudentGradeAvgPerMPC(query);
        //    }
        //}

        //public IList<StudentGradeAvgPerClass> GetStudentsGradePerClass(Guid teacherId, Guid schoolYearId)
        //{
        //    using (var uow = Read())
        //    {
        //        var query = QueryFiltering(new GradingStatisticQuery
        //            {
        //                TeacherId = teacherId,
        //                SchoolYearId = schoolYearId
        //            });
        //        var studentGradeAvgPerMpc = new GradingStatisticDataAccess(uow).CalcStudentGradeAvgPerMPC(query);
        //        var dic = studentGradeAvgPerMpc.GroupBy(x => new Pair<Guid,Guid>(x.MarkingPeriodClass.ClassRef, x.Student.Id))
        //                                       .ToDictionary(x => x.Key, x => x.ToList());
        //        var res = new List<StudentGradeAvgPerClass>();
        //        foreach (var keyValue in dic)
        //        {
        //            res.Add(new StudentGradeAvgPerClass
        //                {
        //                    Student = keyValue.Value.First().Student,
        //                    ClassRef = keyValue.Key.First,
        //                    Avg = (int?)keyValue.Value.Average(x => x.Avg)
        //                });
        //        }
        //        return res;
        //    }
        //}

        //public IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(Guid classId, Guid schoolYearId, List<Guid> markingPeriodIds, Guid? teacherId, Guid? studentId = null)
        //{
        //    using (var uow = Read())
        //    {
        //        var query = QueryFiltering(new GradingStatisticQuery
        //            {
        //                ClassId = classId,
        //                SchoolYearId = schoolYearId,
        //                MarkingPeriodIds = markingPeriodIds,
        //                TeacherId = teacherId,
        //                StudentId = studentId
        //            });
        //        return new GradingStatisticDataAccess(uow).CalcClassGradingPerMp(query);
        //    }
        //}

        //public IList<StudentGradeAvgPerDate> GetStudentGradePerDate(Guid studentId, Guid markingPeriodId, Guid? classId)
        //{
        //    using (var uow = Read())
        //    {
        //        return   new GradingStatisticDataAccess(uow).CalcStudentGradeStatsPerDate(studentId, markingPeriodId, classId, 7);
        //    }
        //}

        //public IList<StudentClassGradeStats> GetStudentClassGradeStats(Guid markingPeriodId, Guid classId, Guid? studentId)
        //{
        //    using (var uow = Read())
        //    {
        //        return new GradingStatisticDataAccess(uow).CalcStudentClassGradeStats(classId, markingPeriodId, studentId, 7);
        //    }
        //}

        //public IList<DepartmentGradeAvg> GetDepartmentGradeAvgPerMp(Guid markingPeriodId, IList<Guid> gradeLevelIds)
        //{
        //    using (var uow = Read())
        //    {
        //        return new GradingStatisticDataAccess(uow).CalcDepartmentGradeAvgPerMp(markingPeriodId, Context.UserId,
        //                                                                               Context.Role.Id, gradeLevelIds);
        //    }
        //}


        //public IList<ClassPersonGradingStats> GetFullGradingStats(Guid markingPeriodId, Guid studentId)
        //{
        //    using (var uow = Read())
        //    {
        //        return new GradingStatisticDataAccess(uow).CalcGradingStats(Context.UserId, Context.Role.Id, studentId, markingPeriodId);
        //    }
        //}

        //public IList<StudentGradingRank> GetStudentGradingRanks(Guid schoolYearId, Guid? studentId, Guid? gradeLevelId, Guid? classId)
        //{
        //    using (var uow = Read())
        //    {
        //        return new GradingStatisticDataAccess(uow).GetStudentGradingRank(Context.UserId, Context.Role.Id, schoolYearId, gradeLevelId, studentId, classId);
        //    }
        //}

        public IList<StudentGradeAvgPerMPC> GetStudentsGradePerMPC(int teacherId, IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
        }

        public IList<StudentGradeAvgPerClass> GetStudentsGradePerClass(int teacherId, int schoolYearId)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(int classId, int schoolYearId, List<int> markingPeriodIds, int? teacherId, int? studentId = null)
        {
            throw new NotImplementedException();
        }

        public IList<StudentGradeAvgPerDate> GetStudentGradePerDate(int studentId, int markingPeriodId, int? classId)
        {
            throw new NotImplementedException();
        }

        public IList<StudentClassGradeStats> GetStudentClassGradeStats(int markingPeriodId, int classId, int? studentId)
        {
            throw new NotImplementedException();
        }

        public IList<DepartmentGradeAvg> GetDepartmentGradeAvgPerMp(int markingPeriodId, IList<int> gradeLevelIds)
        {
            throw new NotImplementedException();
        }

        public IList<ClassPersonGradingStats> GetFullGradingStats(int markingPeriodId, int studentId)
        {
            throw new NotImplementedException();
        }

        public IList<StudentGradingRank> GetStudentGradingRanks(int schoolYearId, int? studentId, int? gradeLevelId, int? classId)
        {
            throw new NotImplementedException();
        }

        public ChalkableGradeBook GetGradeBook(int classId, int gradingPeriodId)
        {
            var stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId, null, gradingPeriodId);
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            return GetGradeBooks(classId, new List<GradingPeriodDetails>{gradingPeriod}, stiGradeBook).First();
        }

        public IList<ChalkableGradeBook> GetGradeBooks(int classId)
        {
            var stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId);
            var schoolYear = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var gradingPeriods = ServiceLocator.GradingPeriodService.GetGradingPeriodsDetails(schoolYear.Id);
            return GetGradeBooks(classId, gradingPeriods, stiGradeBook);
        }

        private IList<ChalkableGradeBook> GetGradeBooks(int classId, IList<GradingPeriodDetails> gradingPeriods, Gradebook gradebook)
        {
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                ClassId = classId,
                RoleId = CoreRoles.STUDENT_ROLE.Id
            });
            var annQuery = new AnnouncementsQuery {ClassId = classId};
            if (gradingPeriods.Count == 1)
            {
                annQuery.FromDate = gradingPeriods.First().StartDate;
                annQuery.ToDate = gradingPeriods.First().EndDate;
            }
            var anns = ServiceLocator.AnnouncementService.GetAnnouncementsComplex(annQuery, gradebook.Activities.ToList());   
            return gradingPeriods.Select(gradingPeriod => BuildGradeBook(gradebook, gradingPeriod, anns, students)).ToList();
        }

        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod,
                                                  IList<AnnouncementComplex> anns, IList<Person> students)
        {
            var gradeBook = new ChalkableGradeBook
            {
                GradingPeriod = gradingPeriod,
                Students = students
            };
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                            && x.Date <= gradingPeriod.EndDate).ToList();
            var stAvgs = stiGradeBook.StudentAverages.Where(x => x.IsGradingPeriodAverage
                && gradingPeriod.Id == x.GradingPeriodId).ToList();
            stAvgs = stAvgs.Where(x => x.Score.HasValue).ToList();
            if (stAvgs.Count > 0)
                gradeBook.Avg = (int)stAvgs.Average(x => x.Score != null ? x.Score.Value : 0);

            var annsDetails = new List<AnnouncementDetails>();
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.SisActivityId == activity.Id);
                var annDetails = new AnnouncementDetails
                {
                    Id = ann.Id,
                    ClassName = ann.ClassName,
                    Title = ann.Title,
                    StudentAnnouncements = new List<StudentAnnouncementDetails>()
                };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(annDetails, activity);
                var scores = stiGradeBook.Scores.Where(x => x.ActivityId == activity.Id).ToList();
                foreach (var score in scores)
                {
                    var stAnn = new StudentAnnouncementDetails
                    {
                        AnnouncementId = ann.Id,
                        ClassId = ann.ClassRef.Value,
                        Student = students.First(x => x.Id == score.StudentId)
                    };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    annDetails.StudentAnnouncements.Add(stAnn);
                }
                annsDetails.Add(annDetails);
            }
            gradeBook.Announcements = annsDetails;
            return gradeBook;
        }



    }
}
