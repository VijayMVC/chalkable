using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiGradeBookStorage:BaseDemoStorage<int, Gradebook>
    {
        public DemoStiGradeBookStorage(DemoStorage storage)
            : base(storage)
        {

        }

        public Gradebook Calculate(int classId, int gradingPeriodId)
        {
            var gb = data.First(x => x.Value.SectionId == classId).Value;
            return PrepareGradeBook(classId, gb);
        }

        private Gradebook PrepareGradeBook(int classId, Gradebook gb)
        {
            gb.Activities = Storage.StiActivityStorage.GetAll().Where(x => x.SectionId == classId);
            var activityIds = gb.Activities.Select(x => x.Id).ToList();
            gb.Scores = Storage.StiActivityScoreStorage.GetAll().Where(x => activityIds.Contains(x.ActivityId));
            return gb;
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId, int? classAnnouncementType, int gradingPeriodId, int? standardId)
        {
            var gradeBooks = data.Select(x => x.Value);

            gradeBooks = gradeBooks.Where(x => x.SectionId == classId);

            if (classAnnouncementType.HasValue)
            {
                gradeBooks = gradeBooks.Where(gb => gb.Activities.Count(x => x.CategoryId == classAnnouncementType.Value) > 0);
            }

            if (standardId.HasValue)
            {
                gradeBooks =
                    gradeBooks.Where(
                        gb =>
                            gb.Activities.Count(x => x.Standards.Select(y => y.Id).ToList().Contains(standardId.Value)) >
                            0);
            }




            var gradeBook = gradeBooks.FirstOrDefault() ?? new Gradebook()
            {
                SectionId = classId,
                Scores = new List<Score>(),
                Options = new ClassroomOption(),
                Activities = new List<Activity>(),
                StudentAverages = new List<StudentAverage>()
            };
            return gradeBook;
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetGradebookComments(int schoolYearId, int teacherId)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
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
                        GradingPeriodId = 1
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 2
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 3
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 4
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
                        GradingPeriodId = 1
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 2
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 3
                    },

                    new StudentAverage()
                    {
                        CalculatedNumericAverage = 100,
                        EnteredNumericAverage = 100,
                        IsGradingPeriodAverage = true,
                        GradingPeriodId = 4
                    }
                }
            };

            data.Add(GetNextFreeId(), gb1);
            data.Add(GetNextFreeId(), gb2);
        }

        public StudentAverage UpdateStudentAverage(int classId, StudentAverage studentAverage)
        {
            throw new NotImplementedException();
        }

        public void PostGrades(int classId, int? gradingPeriodId)
        {
            throw new NotImplementedException();
        }
    }
}
