using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStatisticService
    {
        Task<ChalkableGradeBook> GetGradeBook(int classId, GradingPeriod gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true);
        IList<string> GetGradeBookComments(int schoolYearId, int teacherId);
        Task<TeacherClassGrading> GetClassGradingSummary(int classId, GradingPeriod gradingPeriod);
        void PostGradebook(int classId, int? gradingPeriodId);
        
        IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId); 
        ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue, bool exempt, IList<ChalkableStudentAverageComment> comments, string note);
        Task<IList<ShortClassGradesSummary>> GetClassesGradesSummary(int teacherId, GradingPeriod gradingPeriod);
        Task<FinalGradeInfo> GetFinalGrade(int classId, GradingPeriod gradingPeriod);
        void PostStandards(int classId, int? gradingPeriodId);
        StudentGrading GetStudentGradingSummary(int schoolYearId, int studentId);
        StudentGradingDetails GetStudentGradingDetails(int schoolYearId, int studentId, int gradingPeriodId);
    }
    public class GradingStatisticService : SisConnectedService, IGradingStatisticService
    {
        public GradingStatisticService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public async Task<ChalkableGradeBook> GetGradeBook(int classId, GradingPeriod gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
            Task<Gradebook> stiGradeBook;
            var isTeacherClass = DoRead(u => new ClassTeacherDataAccess(u).Exists(classId, Context.PersonId));
            if (needsReCalculate && GradebookSecurity.CanReCalculateGradebook(Context, isTeacherClass))
                stiGradeBook = ConnectorLocator.GradebookConnector.Calculate(classId, gradingPeriod.Id);
            else
            {
                stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId, classAnnouncementType, gradingPeriod.Id, standardId);     
            }
            Trace.WriteLine("GetGradeBooks" + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            return GetGradeBooks(classId, gradingPeriod, await stiGradeBook);
        }

        private ChalkableGradeBook GetGradeBooks(int classId, GradingPeriod gradingPeriod, Gradebook gradebook)
        {
            int mpId = gradingPeriod.MarkingPeriodRef;
            var classRoomOptions = gradebook.Options;
            var students = ServiceLocator.StudentService.GetClassStudents(classId, mpId, classRoomOptions == null || classRoomOptions.IncludeWithdrawnStudents ? (bool?)null : true);

            var activities = gradebook.Activities
                     .Where(x => x.SectionId == classId)
                     .Where(x => x.Date >= gradingPeriod.StartDate)
                     .Where(x => x.Date <= gradingPeriod.EndDate).ToList();
            var activitiesIds = activities.Select(x => x.Id).ToList();

            var anns = ServiceLocator.ClassAnnouncementService.GetByActivitiesIds(activitiesIds);
            DoUpdate(u=> anns = ClassAnnouncementService.MergeAnnouncementsWithActivities(ServiceLocator, u, anns, activities));

            return BuildGradeBook(gradebook, gradingPeriod, anns, students);
        }


        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod, IList<AnnouncementComplex> anns, IList<StudentDetails> students)
        {
            var gradeBook = new ChalkableGradeBook
            {
                GradingPeriod = gradingPeriod,
                Averages = stiGradeBook.StudentAverages.Select(ChalkableStudentAverage.Create).ToList(),
                Students = students,
                Options = stiGradeBook.Options != null ? ChalkableClassOptions.Create(stiGradeBook.Options) : null
            };
            var includeWithdrawnStudents = gradeBook.Options != null && gradeBook.Options.IncludeWithdrawnStudents;
            
            //Preapred List Of Announcement Info
            Trace.WriteLine("PrepareAnnounceemntDetailsForGradeBook " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            gradeBook.Announcements = PrepareAnnounceemntDetailsForGradeBook(stiGradeBook, gradingPeriod, anns, students, includeWithdrawnStudents);

            //prepare students score
            var stiScores = stiGradeBook.Scores;
            if (!includeWithdrawnStudents)
                stiScores = stiScores.Where(x => !x.Withdrawn).ToList();

            if (stiScores.Any())
                gradeBook.Students = gradeBook.Students.Where(s => stiScores.Any(score => score.StudentId == s.Id)).ToList();
     
            gradeBook.Students = gradeBook.Students
                                    .OrderBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
                                    .ThenBy(x => x.FirstName, StringComparer.OrdinalIgnoreCase).ToList();

            if (stiGradeBook.StudentTotalPoints != null)
            {
                var totalPoints = stiGradeBook.StudentTotalPoints.Where(x => x.GradingPeriodId == gradingPeriod.Id).ToList();
                gradeBook.StudentTotalPoints = StudentTotalPoint.Create(totalPoints);
            }
            return gradeBook;
        }

        private IList<AnnouncementDetails> PrepareAnnounceemntDetailsForGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod
            , IList<AnnouncementComplex> anns, IList<StudentDetails> students, bool inludeWithdrawnStudent)
        {
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                           && x.Date <= gradingPeriod.EndDate && x.IsScored).ToList();
            var annsDetails = new List<AnnouncementDetails>();
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(stiGradeBook.SectionId, null);
            var alternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores();
            foreach (var activity in activities)
            {
                var ann = anns.FirstOrDefault(x => x.ClassAnnouncementData.SisActivityId == activity.Id);
                if (ann == null)
                    throw new ChalkableException(string.Format("No announcements with sis activity id = {0}", activity.Id));
                var annDetails = new AnnouncementDetails
                {
                    Id = ann.Id,
                    AnnouncementData = ann.ClassAnnouncementData,
                    Title = ann.Title,
                    StudentAnnouncements = new List<StudentAnnouncementDetails>(),
                    IsOwner = classTeachers.Any(x => x.PersonRef == Context.PersonId)
                };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(annDetails, activity);
                var scores = stiGradeBook.Scores.Where(x => x.ActivityId == activity.Id).ToList();
                if (!inludeWithdrawnStudent) scores = scores.Where(x => !x.Withdrawn).ToList();
                foreach (var score in scores)
                {
                    var student = students.FirstOrDefault(x => x.Id == score.StudentId);
                    if(student == null) continue;
                    var stAnn = new StudentAnnouncementDetails
                    {
                        AnnouncementId = ann.Id,
                        ClassId = ann.ClassAnnouncementData.ClassRef,
                        Student = student,
                    };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    if (stAnn.AlternateScoreId.HasValue)
                        stAnn.AlternateScore = alternateScores.FirstOrDefault(x => x.Id == stAnn.AlternateScoreId.Value);
                    annDetails.StudentAnnouncements.Add(stAnn);
                }
                annDetails.StudentAnnouncements = annDetails.StudentAnnouncements.OrderBy(x => x.Student.LastName).ThenBy(x => x.Student.FirstName).ToList();
                annsDetails.Add(annDetails);
            }
            return annsDetails;
        } 

        public IList<string> GetGradeBookComments(int schoolYearId, int teacherId)
        {
            return ConnectorLocator.GradebookConnector.GetGradebookComments(schoolYearId, teacherId);
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

        public async Task<IList<ShortClassGradesSummary>> GetClassesGradesSummary(int teacherId, GradingPeriod gradingPeriod)
        {
            var classesDetails = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);

            if (classesDetails.Count == 0)
            {
                return new List<ShortClassGradesSummary>();
            }

            var classesIds = classesDetails.Select(x => x.Id).ToList();
            var stiSectionsGradesTask = ConnectorLocator.GradebookConnector.GetSectionGradesSummary(classesIds, gradingPeriod.Id);
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, gradingPeriod.SchoolYearRef);
            var res = new List<ShortClassGradesSummary>();
            var stiSectionsGrades = await stiSectionsGradesTask;
            foreach (var sectionGrades in stiSectionsGrades)
            {
                var classesDetail = classesDetails.FirstOrDefault(x => x.Id == sectionGrades.SectionId);
                res.Add(ShortClassGradesSummary.Create(sectionGrades, classesDetail, students));
            }
            return res;
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
        
        public IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId)
        {
            var studentAverages = ConnectorLocator.GradebookConnector.GetStudentAverages(classId, gradingPeriodId);
            if (averageId.HasValue)
                studentAverages = studentAverages.Where(x => x.AverageId == averageId).ToList();
            return studentAverages.Select(ChalkableStudentAverage.Create).ToList();
        }


        public async Task<FinalGradeInfo> GetFinalGrade(int classId, GradingPeriod gradingPeriod)
        {
            var averageDashBoardTask = ConnectorLocator.GradebookConnector.GetAveragesDashboard(classId, gradingPeriod.Id);
            var gradeBookTask = GetGradeBook(classId, gradingPeriod, null, null, false);
            var gradeBook = await gradeBookTask;
            var averageDashBoard = await averageDashBoardTask;
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

        public void PostStandards(int classId, int? gradingPeriodId)
        {
            ConnectorLocator.GradebookConnector.PostStandards(classId, gradingPeriodId);
        }

        public StudentGrading GetStudentGradingSummary(int schoolYearId, int studentId)
        {
            var gradingSummaryDashBoard = ConnectorLocator.GradingConnector.GetStudentGradingSummary(schoolYearId,
                studentId);

            return new StudentGrading
            {
                StudentId = gradingSummaryDashBoard.StudentId,
                StudentAverages = gradingSummaryDashBoard.Averages.Select(ChalkableStudentAverage.Create)
            };
        }

        public StudentGradingDetails GetStudentGradingDetails(int schoolYearId, int studentId, int gradingPeriodId)
        {
            var gradingDetailsDashboard = ConnectorLocator.GradingConnector.GetStudentGradingDetails(schoolYearId,
                studentId, gradingPeriodId);

            var mapper = MapperFactory.GetMapper<StudentAnnouncement, Score>();
            var studentAnnouncements = new List<StudentAnnouncement>();
            foreach (var score in gradingDetailsDashboard.Scores)
            {
                var studentAnn = new StudentAnnouncement();
                mapper.Map(studentAnn, score);
                studentAnnouncements.Add(studentAnn);
            }

            return new StudentGradingDetails()
            {
                GradingPeriodId = gradingPeriodId,
                StudentAnnouncements = studentAnnouncements
            };
        }
    }
}
