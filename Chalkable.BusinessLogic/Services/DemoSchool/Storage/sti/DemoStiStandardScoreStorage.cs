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
            return new Gradebook()
            {
                SectionId = classId,
                Activities = new List<Activity>(),
                Options = new ClassroomOption(),
                Scores = new List<Score>(),
                StudentAverages = new List<StudentAverage>()
            };
        }

        public Gradebook GetBySectionAndGradingPeriod(int classId, int? classAnnouncementType, int gradingPeriodId, int? standardId)
        {
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
            throw new NotImplementedException();
        }

        public StudentAverage UpdateStudentAverage(int classId, StudentAverage studentAverage)
        {
            throw new NotImplementedException();
        }
    }
}
