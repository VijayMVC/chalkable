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
            gb.Activities = Storage.StiActivityStorage.GetAll().Where(x => x.SectionId == classId);
            var activityIds = gb.Activities.Select(x => x.Id).ToList();
            gb.Scores = Storage.StiActivityScoreStorage.GetAll().Where(x => activityIds.Contains(x.ActivityId));
            return gb;
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId, int? classAnnouncementType, int gradingPeriodId, int? standardId)
        {
            var gradeBooks = data.Select(x => x.Value).ToList();

            //if (classAnnouncementType.HasValue)
            //{
              //  gradeBooks = gradeBooks.Where(x => x.)
            //}
            //return data.Where(x => x.Value.SectionId == classId)
            throw new NotImplementedException();
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
                Scores = new List<Score>()
                {
                   
                },
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
