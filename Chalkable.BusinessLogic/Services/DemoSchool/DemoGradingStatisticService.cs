using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoGradingStatisticService : DemoSisConnectedService, IGradingStatisticService
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


        public ChalkableGradeBook GetGradeBook(int classId, int gradingPeriodId, int? standardId = null, int? classAnnouncementType = null, bool needsReCalculate = true)
        {
          
            Gradebook stiGradeBook = null;
            if (needsReCalculate)
                stiGradeBook = Storage.StiGradeBookStorage.Calculate(classId, gradingPeriodId);
            if (!needsReCalculate || standardId.HasValue || classAnnouncementType.HasValue)
            {
                stiGradeBook = Storage.StiGradeBookStorage.GetBySectionAndGradingPeriod(classId, classAnnouncementType
                , gradingPeriodId, standardId);
            }
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            return GetGradeBooks(classId, new List<GradingPeriodDetails> { gradingPeriod }, stiGradeBook).First();
        }

        public IList<ChalkableGradeBook> GetGradeBooks(int classId)
        {
            var stiGradeBook = Storage.StiGradeBookStorage.GetBySectionAndGradingPeriod(classId);
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
            var annQuery = new AnnouncementsQuery { ClassId = classId };
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
                gradeBook.Students = new List<Person>();
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
            , IList<AnnouncementComplex> anns, IList<Person> students)
        {
            var activities = stiGradeBook.Activities.Where(x => x.Date >= gradingPeriod.StartDate
                                                          && x.Date <= gradingPeriod.EndDate && x.IsScored).ToList();
            var annsDetails = new List<AnnouncementDetails>();
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
                    IsOwner = ann.PrimaryTeacherRef == Context.UserLocalId
                };
                MapperFactory.GetMapper<AnnouncementDetails, Activity>().Map(annDetails, activity);
                var scores = stiGradeBook.Scores.Where(x => x.ActivityId == activity.Id).ToList();
                foreach (var score in scores)
                {
                    var stAnn = new StudentAnnouncementDetails
                    {
                        AnnouncementId = ann.Id,
                        ClassId = ann.ClassRef,
                        Student = students.First(x => x.Id == score.StudentId)
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

        public void PostGradebook(int classId, int? gradingPeriodId)
        {
            Storage.StiGradeBookStorage.PostGrades(classId, gradingPeriodId);
        }

        
        private double? CalculateAvgByAnnTypes(IEnumerable<GradedClassAnnouncementType> classAnnouncementTypes)
        {
            var res = classAnnouncementTypes.Where(classAnnType => classAnnType.Avg.HasValue).ToList();
            if (res.Count > 0)
                return res.Average(classAnnType => (double)classAnnType.Percentage * classAnnType.Avg.Value / 100);
            return null;
        }


        public ChalkableStudentAverage UpdateStudentAverage(int classId, int studentId, int averageId, int? gradingPeriodId, string averageValue,bool exempt, IList<ChalkableStudentAverageComment> comments)
        {
            var studentAverage = new StudentAverage
            {
                AverageId = averageId,
                StudentId = studentId,
                GradingPeriodId = gradingPeriodId,
                EnteredAverageValue = averageValue
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
            studentAverage = Storage.StiGradeBookStorage.UpdateStudentAverage(classId, studentAverage);
            return ChalkableStudentAverage.Create(studentAverage);
        }

        public IList<ShortClassGradesSummary> GetClassesGradesSummary(int teacherId, int gradingPeriodId)
        {
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var classesDetails = ServiceLocator.ClassService.GetClasses(gradingPeriod.SchoolYearRef, (int?)gradingPeriod.MarkingPeriodRef, (int?)teacherId);
            var classesIds = classesDetails.Select(x => x.Id).ToList();


            var stiSectionsGrades = Storage.StiGradeBookStorage.GetSectionGradesSummary(classesIds, gradingPeriodId);
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                RoleId = CoreRoles.STUDENT_ROLE.Id,
                TeacherId = teacherId
            });
            var res = new List<ShortClassGradesSummary>();
            foreach (var sectionGrade in stiSectionsGrades)
            {
                var classesDetail = classesDetails.FirstOrDefault(x => x.Id == sectionGrade.SectionId);
                res.Add(ShortClassGradesSummary.Create(sectionGrade, classesDetail, students));
            }
            return res;
        }
    }
}
