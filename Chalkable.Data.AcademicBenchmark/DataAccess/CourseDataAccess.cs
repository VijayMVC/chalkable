using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class CourseDataAccess : DataAccessBase<Course, Guid>
    {
        public CourseDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Course> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, bool forTopics = false)
        {
            var @params = new Dictionary<string, object>
            {
                ["authorityId"] = authorityId,
                ["documentId"] = documentId,
                ["subjectDocId"] = subjectDocId,
                ["gradeLevelCode"] = gradeLevelCode,
                ["forTopics"] = forTopics
            };

            return ExecuteStoredProcedureList<Course>("spGetCourses", @params);
        } 
    }
}
