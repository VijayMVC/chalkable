using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiGradeBookStorage:BaseDemoIntStorage<Gradebook>
    {
        public DemoStiGradeBookStorage(DemoStorage storage)
            : base(storage, null, true)
        {

        }

        public Gradebook Calculate(int classId, int gradingPeriodId)
        {
            var gb = data.First(x => x.Value.SectionId == classId && x.Value.StudentAverages.Select(y => y.GradingPeriodId).ToList().Contains(gradingPeriodId)).Value;
            return PrepareGradeBook(classId, gb);
        }

        private Gradebook PrepareGradeBook(int classId, Gradebook gb)
        {
            gb.Activities = Storage.StiActivityStorage.GetAll().Where(x => x.SectionId == classId);
            var activityIds = gb.Activities.Select(x => x.Id).ToList();
            gb.Scores = Storage.StiActivityScoreStorage.GetAll().Where(x => activityIds.Contains(x.ActivityId));
            return gb;
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId, int? classAnnouncementType = null, int? gradingPeriodId = null, int? standardId = null)
        {
            var gradeBooks = data.Select(x => x.Value);

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


            var gbOld = gradeBooks.FirstOrDefault();
            
            var gradeBook = new Gradebook()
            {
                SectionId = classId,
                Scores = gbOld != null ? gbOld.Scores : new List<Score>(),
                Options = gbOld != null ? gbOld.Options : new ClassroomOption(),
                Activities = gbOld != null ? gbOld.Activities : new List<Activity>(),
                StudentAverages = gbOld != null ? gbOld.StudentAverages : new List<StudentAverage>()
                {
                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = gradingPeriodId,
                        StudentId = DemoSchoolConstants.FirstStudentId
                    },
                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = gradingPeriodId,
                        StudentId = DemoSchoolConstants.SecondStudentId
                    },
                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = gradingPeriodId,
                        StudentId = DemoSchoolConstants.ThirdStudentId
                    }
                }
            };


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

        public IList<string> GetGradebookComments(int schoolYearId, int teacherId)
        {
            return new List<string>();
        }

        public void Setup()
        {
            for (var i = 0; i < 4; ++i)
            {
                var gb1 = new Gradebook()
                {
                    SectionId = 1,
                    Activities = new List<Activity>(),
                    Options = new ClassroomOption(),
                    Scores = new List<Score>(),
                    StudentAverages = new List<StudentAverage>()
                    {
                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.FirstStudentId
                        },
                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.SecondStudentId
                        },
                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.ThirdStudentId
                        }
                    }
                };

                var gb2 = new Gradebook()
                {
                    SectionId = 2,
                    Activities = new List<Activity>(),
                    Options = new ClassroomOption(),
                    Scores = new List<Score>(),
                    StudentAverages = new List<StudentAverage>()
                    {
                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.FirstStudentId
                        },

                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.SecondStudentId
                        },

                        new StudentAverage()
                        {
                            CalculatedNumericAverage = 100,
                            EnteredNumericAverage = 100,
                            IsGradingPeriodAverage = true,
                            GradingPeriodId = i + 1,
                            StudentId = DemoSchoolConstants.ThirdStudentId
                        }
                    }
                };

                Add(gb1);
                Add(gb2);
            }
         
            
        }

        public StudentAverage UpdateStudentAverage(int classId, StudentAverage studentAverage)
        {
            var gb = data.First(x => x.Value.SectionId == classId).Value;

            var avgs = gb.StudentAverages.ToList();

            var id = -1;

            for (var i = 0; i < avgs.Count; ++i)
            {
                var avg = avgs[i];
                if (avg.AverageId == studentAverage.AverageId)
                {
                    id = i;
                    break;
                }
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

        public void PostGrades(int classId, int? gradingPeriodId)
        {
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
                var students = new List<StudentSectionGradesSummary>();
                foreach (var studentAvg in gb.StudentAverages)
                {
                   students.Add(new StudentSectionGradesSummary
                   {
                       SectionId = gb.SectionId,
                       Average = studentAvg.CalculatedNumericAverage,
                       Exempt = false,
                       StudentId = studentAvg.StudentId
                   });
                }
                ss.Students = students;

                res.Add(ss);
            }
            return res;
        }
    }
}
