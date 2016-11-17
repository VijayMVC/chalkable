using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradeBookStorage : BaseDemoIntStorage<Gradebook>
    {
        public DemoGradeBookStorage()
            : base(null, true)
        {

        }

    }

    public class DemoGradingStatisticService : DemoSchoolServiceBase, IGradingStatisticService
    {
        private DemoGradeBookStorage GradeBookStorage { get; set; }
        public DemoGradingStatisticService(IServiceLocatorSchool serviceLocator): base(serviceLocator)
        {
            GradeBookStorage = new DemoGradeBookStorage();
        }

        public async Task<ChalkableGradeBook> GetGradeBook(int classId, GradingPeriod gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
            var stiGradeBook = GetBySectionAndGradingPeriod(classId, classAnnouncementType, gradingPeriod.Id, standardId);
            return GetGradeBooks(classId, gradingPeriod, stiGradeBook);
        }

        public IList<string> GetGradebookComments(int schoolYearId, int teacherId)
        {
            return new List<string>();
        }

        public StudentAverage UpdateStudentAverage(StudentAverage studentAverage)
        {
            var gb = GetBySectionAndGradingPeriod(studentAverage.SectionId, null, studentAverage.GradingPeriodId);

            var avgs = gb.StudentAverages.ToList();

            var id = -1;

            for (var i = 0; i < avgs.Count; ++i)
            {
                var avg = avgs[i];
                if (avg.SectionId != studentAverage.SectionId || avg.StudentId != studentAverage.StudentId ||
                    avg.GradingPeriodId != studentAverage.GradingPeriodId) continue;
                id = i;
            }

            if (id == -1)
            {
                avgs.Add(studentAverage);
            }
            else
            {
                avgs[id] = studentAverage;
            }
            gb.StudentAverages = avgs;

            return studentAverage;
        }

        public IEnumerable<SectionGradesSummary> GetSectionGradesSummary(List<int> classesIds, int gradingPeriodId)
        {
            var res = new List<SectionGradesSummary>();

            foreach (var classId in classesIds)
            {
                var gb = GetBySectionAndGradingPeriod(classId, null, gradingPeriodId);
                var ss = new SectionGradesSummary()
                {
                    SectionId = gb.SectionId
                };
                var students = (from studentAvg in gb.Scores
                                select new StudentSectionGradesSummary
                                {
                                    SectionId = gb.SectionId,
                                    Average = studentAvg.NumericScore,
                                    Exempt = false,
                                    StudentId = studentAvg.StudentId
                                }).ToList();
                ss.Students = students;

                res.Add(ss);
            }
            return res;
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId, int? classAnnouncementType = null, int? gradingPeriodId = null, int? standardId = null)
        {
            var gradeBooks = GradeBookStorage.GetData().Select(x => x.Value);

            gradeBooks = gradeBooks.Where(x => x.SectionId == classId);

            if (classAnnouncementType.HasValue)
            {
                gradeBooks = gradeBooks.Where(gb => gb.Activities.Count(x => x.CategoryId == classAnnouncementType.Value) > 0);
            }

            if (standardId.HasValue)
            {
                gradeBooks = gradeBooks
                    .Where(gb => gb.Activities.Count(x => x.Standards != null && x.Standards.Select(y => y.Id).ToList().Contains(standardId.Value)) > 0);
            }

            if (gradingPeriodId.HasValue)
            {
                gradeBooks =
                    gradeBooks.Where(
                        x => x.StudentAverages.Select(y => y.GradingPeriodId).ToList().Contains(gradingPeriodId));
            }
           
            var gradeBook = gradeBooks.First();
            //TODO : impl this
            //gradeBook.Activities = ((DemoAnnouncementService)ServiceLocator.ClassAnnouncementService).GetActivitiesForClass(classId);
            var activityIds = gradeBook.Activities.Select(x => x.Id).ToList();
            gradeBook.Scores = ((DemoStudentAnnouncementService) ServiceLocator.StudentAnnouncementService).GetActivityScores(activityIds);

            if (classAnnouncementType.HasValue)
            {
                gradeBook.Activities = gradeBook.Activities.Where(x => x.CategoryId == classAnnouncementType.Value);
            }
            if (standardId.HasValue)
            {
                gradeBook.Activities = gradeBook.Activities.Where(x => x.Standards != null && x.Standards.Select(y => y.Id).ToList().Contains(standardId.Value));
            }
            return gradeBook;
        }

        private ChalkableGradeBook GetGradeBooks(int classId, GradingPeriod gradingPeriod, Gradebook gradebook)
        {
            var students = ServiceLocator.StudentService.GetClassStudents(classId, gradingPeriod.MarkingPeriodRef);
            var annQuery = new ClassAnnouncementsQuery
            {
                ClassId = classId,
                FromDate = gradingPeriod.StartDate,
                ToDate = gradingPeriod.EndDate
            };
            //TODO : rewrite this 
            throw new NotImplementedException();
            //var anns = ServiceLocator.ClassAnnouncementService.GetAnnouncementsComplex(annQuery, null, gradebook.Activities.ToList());
            //return BuildGradeBook(gradebook, gradingPeriod, anns, students);
        }

        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod,
                                                  IList<AnnouncementComplex> anns, IList<Student> students)
        {
            var gradeBook = new ChalkableGradeBook
            {
                GradingPeriod = gradingPeriod,
                Options = ChalkableClassOptions.Create(stiGradeBook.Options),
                Averages = stiGradeBook.StudentAverages.Select(ChalkableStudentAverage.Create).ToList(),
                Students = students,
                Announcements = PrepareAnnouncementDetailsForGradeBook(stiGradeBook, gradingPeriod, anns, students)
            };
            if (!stiGradeBook.Options.IncludeWithdrawnStudents)
            {
                gradeBook.Students = new List<Student>();
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
            , IList<AnnouncementComplex> anns, IList<Student> students)
        {
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                           && x.Date <= gradingPeriod.EndDate && x.IsScored).ToList();
            var annsDetails = new List<AnnouncementDetails>();
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(stiGradeBook.SectionId, null);
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == activity.Id);
                var annDetails = new AnnouncementDetails
                {
                    Id = ann.Id,
                    Title = ann.Title,
                    AnnouncementData = ann.ClassAnnouncementData,
                    StudentAnnouncements = new List<StudentAnnouncementDetails>(),
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
                        ClassId = ann.ClassAnnouncementData.ClassRef,
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
            return GetGradebookComments(schoolYearId, teacherId);
        }

        public async Task<TeacherClassGrading> GetClassGradingSummary(int classId, GradingPeriod gradingPeriod)
        {
            var gradeBook = await ServiceLocator.GradingStatisticService.GetGradeBook(classId, gradingPeriod);
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
                SectionId = classId,
                IsGradingPeriodAverage = true
            };

            decimal numericScore = -1;
            var isAlphaGrade = !decimal.TryParse(averageValue, out numericScore);

            if (isAlphaGrade)
            {
                var alphaGrade = ServiceLocator.AlphaGradeService.GetAlphaGrades().FirstOrDefault(x => x.Name.ToLowerInvariant() == averageValue);
                if (alphaGrade != null)
                {
                    var gradingScaleRange = ((DemoGradingScaleService)ServiceLocator.GradingScaleService).GetByAlphaGradeId(alphaGrade.Id);

                    if (gradingScaleRange != null)
                        numericScore = gradingScaleRange.AveragingEquivalent;

                    studentAverage.EnteredAlphaGradeId = alphaGrade.Id;
                    studentAverage.EnteredAlphaGradeName = alphaGrade.Name;
                    studentAverage.CalculatedAlphaGradeId = alphaGrade.Id;
                    studentAverage.CalculatedAlphaGradeName = alphaGrade.Name;
                }
            }

            studentAverage.EnteredNumericAverage = numericScore;
            studentAverage.CalculatedNumericAverage = numericScore;

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
            studentAverage = UpdateStudentAverage(studentAverage);
            return ChalkableStudentAverage.Create(studentAverage);
        }

        public async Task<IList<ShortClassGradesSummary>> GetClassesGradesSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var classesDetails = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);
            var classesIds = classesDetails.Select(x => x.Id).ToList();
            var stiSectionsGrades = GetSectionGradesSummary(classesIds, gradingPeriod.Id);
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

        private IList<StudentDisciplineSummary> GetStudentDisciplineSummaries(IEnumerable<Student> students, int classId, DateTime startDate, DateTime endDate)
        {
            var classDisciplines = ((DemoDisciplineService)ServiceLocator.DisciplineService).GetSectionDisciplineSummary(classId, startDate, endDate);
            var result = new List<StudentDisciplineSummary>();

            var studentDetailsList = students as IList<Student> ?? students.ToList();
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

        private AverageDashboard GetAveragesDashboard(ChalkableGradeBook chlkGradeBook, int classId)
        {
            var startDate = new DateTime(DateTime.Today.Year, 1, 1);
            var endDate = DateTime.Today;
            var attendance = ((DemoAttendanceService)ServiceLocator.AttendanceService).GetSectionAttendanceSummary(
                new List<int> { classId }, startDate, endDate).First();

            var attendances = chlkGradeBook.Students.Select(x => new StudentTotalSectionAttendance()
            {
                Absences= attendance.Students.Where(y => y.StudentId == x.Id).Select(y => y.Absences ?? 0).FirstOrDefault(),
                Tardies = attendance.Students.Where(y => y.StudentId == x.Id).Select(y => y.Tardies ?? 0).FirstOrDefault(),
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

        public async Task<FinalGradeInfo> GetFinalGrade(int classId, GradingPeriod gradingPeriod)
        {
        
            var gradeBook = await GetGradeBook(classId, gradingPeriod, null, null, false);
            var averageDashBoard = GetAveragesDashboard(gradeBook, classId);
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

        public void PostStandards(int classId, int? gradingPeriodId)
        {

        }

        public async Task<StudentGrading> GetStudentGradingSummary(int schoolYearId, int studentId)
        {
            throw new NotImplementedException();
        }

        public StudentGradingDetails GetStudentGradingDetails(int schoolYearId, int studentId, int gradingPeriodId)
        {
            throw new NotImplementedException();
        }

        public void AddGradeBook(Gradebook gradebook)
        {
            GradeBookStorage.Add(gradebook);
        }
    }
}
