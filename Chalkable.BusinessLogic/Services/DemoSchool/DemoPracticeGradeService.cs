using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPracticeGradeService : DemoSchoolServiceBase, IPracticeGradeService
    {
        public DemoPracticeGradeService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score)
        {
            throw new NotImplementedException();
        }

        public IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId)
        {
            throw new NotImplementedException();
        }
    }
}
