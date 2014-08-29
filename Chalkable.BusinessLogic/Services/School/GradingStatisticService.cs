using System;
using System.Linq;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
        

        //new services
        IList<ChalkableGradeBook> GetGradeBooks(int classId);
        ChalkableGradeBook GetGradeBook(int classId, int gradingPeriodId, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true);
        IList<string> GetGradeBookComments(int schoolYearId, int teacherId);
        TeacherClassGrading GetClassGradingSummary(int classId, int gradingPeriodId);
        void PostGradebook(int classId, int? gradingPeriodId);
        IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId); 
        ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue, bool exempt, IList<ChalkableStudentAverageComment> comments, string note);
        IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId);
        FinalGradeInfo GetFinalGrade(int classId, int gradingPeriodId);
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

        public ChalkableGradeBook GetGradeBook(int classId, int gradingPeriodId, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
            Gradebook stiGradeBook = null;
            if(needsReCalculate)
                stiGradeBook = ConnectorLocator.GradebookConnector.Calculate(classId, gradingPeriodId);
            if (!needsReCalculate || standardId.HasValue || classAnnouncementType.HasValue)
            {
                stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId, classAnnouncementType
                , gradingPeriodId, standardId);
            }
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            return GetGradeBooks(classId, new List<GradingPeriodDetails>{gradingPeriod}, stiGradeBook).First();
        }

        public IList<ChalkableGradeBook> GetGradeBooks(int classId)
        {
            var stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId);
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            var gradingPeriods = ServiceLocator.GradingPeriodService.GetGradingPeriodsDetails(Context.SchoolYearId.Value);
            return GetGradeBooks(classId, gradingPeriods, stiGradeBook);
        }

        private IList<ChalkableGradeBook> GetGradeBooks(int classId, IList<GradingPeriodDetails> gradingPeriods, Gradebook gradebook)
        {
            var annQuery = new AnnouncementsQuery {ClassId = classId};
            int? mpId = null;
            if (gradingPeriods.Count == 1)
            {
                annQuery.FromDate = gradingPeriods.First().StartDate;
                annQuery.ToDate = gradingPeriods.First().EndDate;
                mpId = gradingPeriods.First().MarkingPeriodRef;
            }

            var students = ServiceLocator.ClassService.GetStudents(classId, gradebook.Options.IncludeWithdrawnStudents ? (bool?)null : true, mpId);
            var anns = ServiceLocator.AnnouncementService.GetAnnouncementsComplex(annQuery, gradebook.Activities.ToList());   
            return gradingPeriods.Select(gradingPeriod => BuildGradeBook(gradebook, gradingPeriod, anns, students)).ToList();
        }

        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod,
                                                  IList<AnnouncementComplex> anns, IList<Person> students)
        {
            var gradeBook = new ChalkableGradeBook
            {
                GradingPeriod = gradingPeriod,
                Options = ChalkableClassOptions.Create(stiGradeBook.Options),
                Averages = stiGradeBook.StudentAverages.Select(ChalkableStudentAverage.Create).ToList(),
                Students = students
            };
            var stAvgs = stiGradeBook.StudentAverages.Where(x => x.IsGradingPeriodAverage
                && gradingPeriod.Id == x.GradingPeriodId).ToList();
            stAvgs = stAvgs.Where(x => x.CalculatedNumericAverage.HasValue || x.EnteredNumericAverage.HasValue).ToList();
            if (stAvgs.Count > 0)
                gradeBook.Avg = (int)stAvgs.Average(x => (x.CalculatedNumericAverage ?? x.EnteredNumericAverage) ?? 0);

            gradeBook.Announcements = PrepareAnnounceemntDetailsForGradeBook(stiGradeBook, gradingPeriod, anns, students);
            if (!stiGradeBook.Options.IncludeWithdrawnStudents)
            {
                gradeBook.Students = new List<Person>();
                foreach (var student in students)
                {
                   var score = stiGradeBook.Scores.FirstOrDefault(x => x.StudentId == student.Id);
                   if(score == null || !score.Withdrawn)
                       gradeBook.Students.Add(student);
                }    
            }
            return gradeBook;
        }

        private IList<AnnouncementDetails> PrepareAnnounceemntDetailsForGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod
            , IList<AnnouncementComplex> anns, IList<Person> students)
        {
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                           && x.Date <= gradingPeriod.EndDate && x.IsScored).ToList();
            var annsDetails = new List<AnnouncementDetails>();
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(stiGradeBook.SectionId, null);
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.SisActivityId == activity.Id);
                if (ann == null)
                    throw new ChalkableException(string.Format("No announcements with sis activity id = {0}", activity.Id));
                var annDetails = new AnnouncementDetails
                {
                    Id = ann.Id,
                    ClassName = ann.ClassName,
                    Title = ann.Title,
                    StudentAnnouncements = new List<StudentAnnouncementDetails>(),
                    PrimaryTeacherRef = ann.PrimaryTeacherRef,
                    IsOwner = classTeachers.Any(x=>x.PersonRef == Context.UserLocalId)
                };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(annDetails, activity);
                var scores = stiGradeBook.Scores.Where(x => x.ActivityId == activity.Id).ToList();
                if (!stiGradeBook.Options.IncludeWithdrawnStudents)
                    scores = scores.Where(x => !x.Withdrawn).ToList();
                var alternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores();
                foreach (var score in scores)
                {
                    var student = students.FirstOrDefault(x => x.Id == score.StudentId);
                    if(student == null) continue;
                    var stAnn = new StudentAnnouncementDetails
                    {
                        AnnouncementId = ann.Id,
                        ClassId = ann.ClassRef,
                        Student = student,
                    };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    if (stAnn.AlternateScoreId.HasValue)
                        stAnn.AlternateScore = alternateScores.FirstOrDefault(x => x.Id == stAnn.AlternateScoreId.Value);
                    annDetails.StudentAnnouncements.Add(stAnn);
                }
                annsDetails.Add(annDetails);
            }
            return annsDetails;
        } 


        public IList<string> GetGradeBookComments(int schoolYearId, int teacherId)
        {
            return ConnectorLocator.GradebookConnector.GetGradebookComments(schoolYearId, teacherId);
        }
        
        public TeacherClassGrading GetClassGradingSummary(int classId, int gradingPeriodId)
        {
            var gradeBook = ServiceLocator.GradingStatisticService.GetGradeBook(classId, gradingPeriodId);
            var gradedCAnnTypes = ServiceLocator.ClassAnnouncementTypeService.CalculateAnnouncementTypeAvg(classId, gradeBook.Announcements);
            return new TeacherClassGrading
                {
                    Announcements = gradeBook.Announcements,
                    AnnouncementTypes = gradedCAnnTypes,
                    GradingPeriod = gradeBook.GradingPeriod,
                    Avg = CalculateAvgByAnnTypes(gradedCAnnTypes)
                };
        }

        private double? CalculateAvgByAnnTypes(IEnumerable<GradedClassAnnouncementType> classAnnouncementTypes)
        {
            var res = classAnnouncementTypes.Where(classAnnType => classAnnType.Avg.HasValue).ToList();
            if(res.Count > 0)
                return res.Average(classAnnType => (double)classAnnType.Percentage*classAnnType.Avg.Value/100);
            return null;
        }

        public void PostGradebook(int classId, int? gradingPeriodId)
        {
            ConnectorLocator.GradebookConnector.PostGrades(classId, gradingPeriodId);
        }


        public ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue, bool exempt, IList<ChalkableStudentAverageComment> comments, string note)
        {
            var studentAverage = new StudentAverage
                {
                    AverageId = averageId,
                    StudentId = studentId,
                    GradingPeriodId = gradingPeriodId,
                    EnteredAverageValue = averageValue,
                    Exempt = exempt
                };
            if (comments != null)
            {
                studentAverage.Comments = new List<StudentAverageComment>();
                foreach (var comment in comments)
                {
                    var stAvgComment = new StudentAverageComment
                        {
                            AverageId = averageId,
                            StudentId = studentId,
                            HeaderId = comment.HeaderId,
                            HeaderText = comment.HeaderText,
                            HeaderSequence = comment.HeaderSequence,
                        };
                    if (comment.GradingComment != null)
                    {
                        stAvgComment.CommentId = comment.GradingComment.Id;
                        stAvgComment.CommentCode = comment.GradingComment.Code;
                        stAvgComment.CommentText = comment.GradingComment.Comment;
                    }
                    studentAverage.Comments.Add(stAvgComment);
                }
            }
            if (note != null)
                studentAverage.ReportCardNote = note;
            studentAverage = ConnectorLocator.GradebookConnector.UpdateStudentAverage(classId, studentAverage);
            return ChalkableStudentAverage.Create(studentAverage);
        }


        public IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId)
        {
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var classesDetails = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, gradingPeriod.MarkingPeriodRef, teacherId, 0, int.MaxValue);
            var classesIds = classesDetails.Select(x => x.Id).ToList();
            var stiSectionsGrades = ConnectorLocator.GradebookConnector.GetSectionGradesSummary(classesIds, gradingPeriodId);
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
                {
                    RoleId = CoreRoles.STUDENT_ROLE.Id,
                    TeacherId = teacherId
                });
            var res = new List<ShortClassGradesSummary>();
            foreach (var sectionGrades in stiSectionsGrades)
            {
                var classesDetail = classesDetails.FirstOrDefault(x => x.Id == sectionGrades.SectionId);
                res.Add(ShortClassGradesSummary.Create(sectionGrades, classesDetail, students));
            }
            return res;
        }

        public IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId)
        {
            var studentAverages = ConnectorLocator.GradebookConnector.GetStudentAverages(classId, gradingPeriodId);
            if (averageId.HasValue)
                studentAverages = studentAverages.Where(x => x.AverageId == averageId).ToList();
            return studentAverages.Select(ChalkableStudentAverage.Create).ToList();
        }


        public FinalGradeInfo GetFinalGrade(int classId, int gradingPeriodId)
        {
            var averageDashBoard = ConnectorLocator.GradebookConnector.GetAveragesDashboard(classId, gradingPeriodId);
            var gradeBook = GetGradeBook(classId, gradingPeriodId);
            gradeBook.Averages = averageDashBoard.StudentAverages.Select(ChalkableStudentAverage.Create).ToList();
            var infractions = ServiceLocator.InfractionService.GetInfractions();
            return new FinalGradeInfo
                {
                    Attendances = FinalStudentAttendance.Create(averageDashBoard.AttendanceSummary),
                    Disciplines = FinalStudentDiscipline.Create(averageDashBoard.DisciplineSummary, infractions),
                    GradeBook = gradeBook,
                    Averages = gradeBook.Averages.GroupBy(x => x.AverageId)
                           .Select(x => ChalkableAverage.Create(x.Key, x.First().AverageName))
                           .ToList()
                };
        }
    }
}
