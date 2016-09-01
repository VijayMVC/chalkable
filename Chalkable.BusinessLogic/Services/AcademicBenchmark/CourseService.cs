using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface ICourseService : IAcademicBenchmarkServiceBase<Course, Guid>
    {
        IList<Course> GetForStandards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode);
        IList<Course> GetForTopics(Guid? subjectDocId);
    }

    public class CourseService : AcademicBenchmarkServiceBase<Course, Guid>, ICourseService
    {
        public CourseService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public IList<Course> GetForStandards(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode)
        {
            if (!authorityId.HasValue && !documentId.HasValue && !subjectDocId.HasValue && !string.IsNullOrWhiteSpace(gradeLevelCode))
                return GetAll();

            return DoRead(u => new CourseDataAccess(u).Get(authorityId, documentId, subjectDocId, gradeLevelCode));
        }

        public IList<Course> GetForTopics(Guid? subjectDocId)
        {
            return DoRead(u => new CourseDataAccess(u).Get(null, null, subjectDocId, null, true));
        } 
    }
}
