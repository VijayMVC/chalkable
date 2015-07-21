using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStatisticService
    {
        ChalkableGradeBook GetGradeBook(int classId, GradingPeriod gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true);
        IList<string> GetGradeBookComments(int schoolYearId, int teacherId);
        TeacherClassGrading GetClassGradingSummary(int classId, GradingPeriod gradingPeriod);
        void PostGradebook(int classId, int? gradingPeriodId);
        
        IList<ChalkableStudentAverage> GetStudentAverages(int classId, int? averageId, int? gradingPeriodId); 
        ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue, bool exempt, IList<ChalkableStudentAverageComment> comments, string note);
        IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId);
        FinalGradeInfo GetFinalGrade(int classId, GradingPeriod gradingPeriod);
        void PostStandards(int classId, int? gradingPeriodId);
    }
    public class GradingStatisticService : SisConnectedService, IGradingStatisticService
    {
        public GradingStatisticService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public ChalkableGradeBook GetGradeBook(int classId, GradingPeriod gradingPeriod, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
            Gradebook stiGradeBook = null;
            Trace.WriteLine("GradebookConnector.Calculate" + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            if (needsReCalculate && GradebookSecurity.CanReCalculateGradebook(Context))
                stiGradeBook = ConnectorLocator.GradebookConnector.Calculate(classId, gradingPeriod.Id);
            else
            {
                stiGradeBook = ConnectorLocator.GradebookConnector.GetBySectionAndGradingPeriod(classId, classAnnouncementType, gradingPeriod.Id, standardId);     
            }
            Trace.WriteLine("GetGradeBooks" + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            return GetGradeBooks(classId, gradingPeriod, stiGradeBook);
        }

        private ChalkableGradeBook GetGradeBooks(int classId, GradingPeriod gradingPeriod, Gradebook gradebook)
        {
            var annQuery = new AnnouncementsQuery {ClassId = classId};
            int mpId = gradingPeriod.MarkingPeriodRef;
            annQuery.FromDate = gradingPeriod.StartDate;
            annQuery.ToDate = gradingPeriod.EndDate;
            var classRoomOptions = gradebook.Options;
            Trace.WriteLine("GetClassStudents " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var students = ServiceLocator.StudentService.GetClassStudents(classId, mpId, classRoomOptions == null || classRoomOptions.IncludeWithdrawnStudents ? (bool?)null : true);
            Trace.WriteLine("GetAnnouncementsComplex " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var anns = ServiceLocator.ClassAnnouncementService.GetAnnouncementsComplex(annQuery, gradebook.Activities.ToList());
            Trace.WriteLine("BuildGradeBook " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            return BuildGradeBook(gradebook, gradingPeriod, anns, students);
        }

        private ChalkableGradeBook BuildGradeBook(Gradebook stiGradeBook, GradingPeriod gradingPeriod, IList<AnnouncementComplex> anns, IList<StudentDetails> students)
        {
            var gradeBook = new ChalkableGradeBook
            {
                GradingPeriod = gradingPeriod,
                Averages = stiGradeBook.StudentAverages.Select(ChalkableStudentAverage.Create).ToList(),
                Students = students
            };
            if (stiGradeBook.Options != null)
                gradeBook.Options = ChalkableClassOptions.Create(stiGradeBook.Options);
            var stAvgs = stiGradeBook.StudentAverages.Where(x => x.IsGradingPeriodAverage
                && gradingPeriod.Id == x.GradingPeriodId).ToList();
            stAvgs = stAvgs.Where(x => x.CalculatedNumericAverage.HasValue || x.EnteredNumericAverage.HasValue).ToList();
            if (stAvgs.Count > 0)
                gradeBook.Avg = (int)stAvgs.Average(x => (x.CalculatedNumericAverage ?? x.EnteredNumericAverage) ?? 0);

            var includeWithdrawnStudents = stiGradeBook.Options != null && stiGradeBook.Options.IncludeWithdrawnStudents;
            Trace.WriteLine("PrepareAnnounceemntDetailsForGradeBook " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            gradeBook.Announcements = PrepareAnnounceemntDetailsForGradeBook(stiGradeBook, gradingPeriod, anns, students, includeWithdrawnStudents);
            if (!includeWithdrawnStudents)
            {
                Trace.WriteLine("includeWithdrawnStudents " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
                gradeBook.Students = new List<StudentDetails>();
                foreach (var student in students)
                {
                   var score = stiGradeBook.Scores.FirstOrDefault(x => x.StudentId == student.Id);
                   if(score == null || !score.Withdrawn)
                       gradeBook.Students.Add(student);
                }    
            }
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
        
        public TeacherClassGrading GetClassGradingSummary(int classId, GradingPeriod gradingPeriod)
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

        public IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId)
        {
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var classesDetails = ServiceLocator.ClassService.GetTeacherClasses(gradingPeriod.SchoolYearRef, teacherId, gradingPeriod.MarkingPeriodRef);

            if (classesDetails.Count == 0)
            {
                return new List<ShortClassGradesSummary>();
            }

            var classesIds = classesDetails.Select(x => x.Id).ToList();
            var stiSectionsGrades = ConnectorLocator.GradebookConnector.GetSectionGradesSummary(classesIds, gradingPeriodId);
            var students = ServiceLocator.StudentService.GetTeacherStudents(teacherId, gradingPeriod.SchoolYearRef);
            var res = new List<ShortClassGradesSummary>();
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


        public FinalGradeInfo GetFinalGrade(int classId, GradingPeriod gradingPeriod)
        {
            var averageDashBoard = ConnectorLocator.GradebookConnector.GetAveragesDashboard(classId, gradingPeriod.Id);
            var gradeBook = GetGradeBook(classId, gradingPeriod, null, null, false);
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
    }
}
