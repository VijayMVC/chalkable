using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoGradingStatisticService : DemoSchoolServiceBase, IGradingStatisticService
    {
        public DemoGradingStatisticService(IServiceLocatorSchool serviceLocator, DemoStorage storage): base(serviceLocator, storage)
        {
        }

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


        public ChalkableGradeBook GetGradeBook(int classId, GradingPeriodDetails gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
          
            Gradebook stiGradeBook = null;
            if (needsReCalculate)
                stiGradeBook = Storage.StiGradeBookStorage.Calculate(classId, gradingPeriod.Id);
            if (!needsReCalculate || standardId.HasValue || classAnnouncementType.HasValue)
            {
                stiGradeBook = Storage.StiGradeBookStorage.GetBySectionAndGradingPeriod(classId, classAnnouncementType
                , gradingPeriod.Id, standardId);
            }
            return GetGradeBooks(classId, gradingPeriod, stiGradeBook);
        }

        private ChalkableGradeBook GetGradeBooks(int classId, GradingPeriodDetails gradingPeriod, Gradebook gradebook)
        {
            var students = ServiceLocator.StudentService.GetClassStudents(classId, gradingPeriod.MarkingPeriodRef);
            var annQuery = new AnnouncementsQuery { ClassId = classId };
            annQuery.FromDate = gradingPeriod.StartDate;
            annQuery.ToDate = gradingPeriod.EndDate;
            var anns = ServiceLocator.AnnouncementService.GetAnnouncementsComplex(annQuery, gradebook.Activities.ToList());
            return BuildGradeBook(gradebook, gradingPeriod, anns, students);
        }

        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod,
                                                  IList<AnnouncementComplex> anns, IList<StudentDetails> students)
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

            gradeBook.Announcements = PrepareAnnouncementDetailsForGradeBook(stiGradeBook, gradingPeriod, anns, students);
            if (!stiGradeBook.Options.IncludeWithdrawnStudents)
            {
                gradeBook.Students = new List<StudentDetails>();
                foreach (var student in students)
                {
                    var score = stiGradeBook.Scores.FirstOrDefault(x => x.StudentId == student.Id);
                    if (score != null && !score.Withdrawn)
                        gradeBook.Students.Add(student);
                }
            }
            return gradeBook;
        }

        private IList<AnnouncementDetails> PrepareAnnouncementDetailsForGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod
            , IList<AnnouncementComplex> anns, IList<StudentDetails> students)
        {
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                           && x.Date <= gradingPeriod.EndDate && x.IsScored).ToList();
            var annsDetails = new List<AnnouncementDetails>();
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(stiGradeBook.SectionId, null);
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.SisActivityId == activity.Id);
                var annDetails = new AnnouncementDetails
                {
                    Id = ann.Id,
                    ClassName = ann.ClassName,
                    Title = ann.Title,
                    StudentAnnouncements = new List<StudentAnnouncementDetails>(),
                    PrimaryTeacherRef = ann.PrimaryTeacherRef,
                    IsOwner = classTeachers.Any(x => x.PersonRef == Context.PersonId)
                };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(annDetails, activity);
                var scores = stiGradeBook.Scores.Where(x => x.ActivityId == activity.Id).ToList();
                if (!stiGradeBook.Options.IncludeWithdrawnStudents)
                    scores = scores.Where(x => !x.Withdrawn).ToList();
                foreach (var score in scores)
                {
                    var student = students.FirstOrDefault(x => x.Id == score.StudentId);
                    if (student == null) continue;
                    var stAnn = new StudentAnnouncementDetails
                    {
                        AnnouncementId = ann.Id,
                        ClassId = ann.ClassRef,
                        Student = student,
                    };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    annDetails.StudentAnnouncements.Add(stAnn);
                }
                annsDetails.Add(annDetails);
            }
            return annsDetails;
        }


        public IList<string> GetGradeBookComments(int schoolYearId, int teacherId)
        {
            return Storage.StiGradeBookStorage.GetGradebookComments(schoolYearId, teacherId);
        }

        public TeacherClassGrading GetClassGradingSummary(int classId, GradingPeriodDetails gradingPeriod)
        {
            var gradeBook = ServiceLocator.GradingStatisticService.GetGradeBook(classId, gradingPeriod);
            var gradedCAnnTypes = ServiceLocator.ClassAnnouncementTypeService.CalculateAnnouncementTypeAvg(classId, gradeBook.Announcements);
            return new TeacherClassGrading
            {
                Announcements = gradeBook.Announcements,
                AnnouncementTypes = gradedCAnnTypes,
                GradingPeriod = gradeBook.GradingPeriod,
                Avg = CalculateAvgByAnnTypes(gradedCAnnTypes)
            };
        }

        public void PostGradebook(int classId, int? gradingPeriodId)
        {
            Storage.StiGradeBookStorage.PostGrades(classId, gradingPeriodId);
        }

        
        private static double? CalculateAvgByAnnTypes(IEnumerable<GradedClassAnnouncementType> classAnnouncementTypes)
        {
            var res = classAnnouncementTypes.Where(classAnnType => classAnnType.Avg.HasValue).ToList();
            if (res.Count > 0)
                return res.Average(classAnnType => (double)classAnnType.Percentage * classAnnType.Avg.Value / 100);
            return null;
        }


        public ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue,bool exempt, IList<ChalkableStudentAverageComment> comments, string note)
        {
            var studentAverage = new StudentAverage
            {
                AverageId = averageId,
                StudentId = studentId,
                GradingPeriodId = gradingPeriodId,
                EnteredAverageValue = averageValue,
                SectionId = classId
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
            studentAverage = Storage.StiGradeBookStorage.UpdateStudentAverage(classId, studentAverage);
            return ChalkableStudentAverage.Create(studentAverage);
        }

        public IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var classesDetails = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, (int?)gradingPeriod.MarkingPeriodRef, (int?)teacherId);
            var classesIds = classesDetails.Select(x => x.Id).ToList();


            var stiSectionsGrades = Storage.StiGradeBookStorage.GetSectionGradesSummary(classesIds, gradingPeriodId);
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, Context.SchoolYearId.Value);
            var res = new List<ShortClassGradesSummary>();
            foreach (var sectionGrade in stiSectionsGrades)
            {
                var classesDetail = classesDetails.FirstOrDefault(x => x.Id == sectionGrade.SectionId);
                res.Add(ShortClassGradesSummary.Create(sectionGrade, classesDetail, students));
            }
            return res;
        }


        public IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId)
        {
            throw new NotImplementedException();
        }

        private IList<StudentDisciplineSummary> GetStudentDisciplineSummaries(IEnumerable<StudentDetails> students, int classId, DateTime startDate, DateTime endDate)
        {
            var classDisciplines = Storage.StiDisciplineStorage.GetSectionDisciplineSummary(classId, startDate, endDate);
            var result = new List<StudentDisciplineSummary>();

            var studentDetailsList = students as IList<StudentDetails> ?? students.ToList();
            foreach (var student in studentDetailsList)
            {
                var infractionsList = classDisciplines.Where(x => x.StudentId == student.Id).SelectMany(x => x.Infractions);

                var groupedInfractions = infractionsList.GroupBy(x => x.Id).Select(y => new
                {
                    Id = y.Key,
                    Count = y.Count()
                });

                result.AddRange(groupedInfractions.Select(groupedInfraction => new StudentDisciplineSummary
                {
                    InfractionId = groupedInfraction.Id, 
                    Occurrences = groupedInfraction.Count,
                    StudentId = student.Id
                }));
            }

            return result;
        } 

        private AverageDashboard GetAveragesDashboard(ChalkableGradeBook chlkGradeBook, int classId, int gradingPeriodId)
        {
            var startDate = new DateTime(DateTime.Today.Year, 1, 1);
            var endDate = DateTime.Today;
            var attendance = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(
                new List<int> { classId }, startDate, endDate).First();

            var attendances = chlkGradeBook.Students.Select(x => new StudentTotalSectionAttendance()
            {
                Absences= attendance.Students.Where(y => y.StudentId == x.Id).Select(y => y.Absences).FirstOrDefault(),
                Tardies = attendance.Students.Where(y => y.StudentId == x.Id).Select(y => y.Tardies).FirstOrDefault(),
                StudentId = x.Id,
                SectionId = classId,
                DaysPresent = attendance.Students.Count(y => y.StudentId == x.Id && y.Absences == 0)
            }).ToList();

            return new AverageDashboard
            {
                AttendanceSummary = attendances,
                DisciplineSummary = GetStudentDisciplineSummaries(chlkGradeBook.Students, classId, startDate, endDate)
            };
        }

        public FinalGradeInfo GetFinalGrade(int classId, GradingPeriodDetails gradingPeriod)
        {
        
            var gradeBook = GetGradeBook(classId, gradingPeriod, null, null, false);
            var averageDashBoard = GetAveragesDashboard(gradeBook, classId, gradingPeriod.Id);
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
