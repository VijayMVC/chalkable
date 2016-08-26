using System;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IAcademicBenchmarkServiceLocator
    {
        UserContext Context { get; }
        IStandardService StandardService { get; }
        IAcademicBenchmarkServiceBase<Document, Guid> DocumentService { get; }
        IAcademicBenchmarkServiceBase<Authority, Guid> AuthorityService { get; }
        IAcademicBenchmarkServiceBase<Course, Guid> CourseService { get; }
        IAcademicBenchmarkServiceBase<GradeLevel, string> GradeLevelService { get; }
        IAcademicBenchmarkServiceBase<StandardDerivative, Guid> StandardDerivativeService { get; }
        IAcademicBenchmarkServiceBase<Subject, string> SubjectService { get; }
        IDbService DbService { get; }
    }

    public class AcademicBenchmarkServiceLocator : IAcademicBenchmarkServiceLocator
    {
        public UserContext Context { get; protected set; }
        public IStandardService StandardService { get; }
        public IAcademicBenchmarkServiceBase<Document, Guid> DocumentService { get; }
        public IAcademicBenchmarkServiceBase<Authority, Guid> AuthorityService { get; }
        public IAcademicBenchmarkServiceBase<Course, Guid> CourseService { get; }
        public IAcademicBenchmarkServiceBase<GradeLevel, string> GradeLevelService { get; }
        public IAcademicBenchmarkServiceBase<StandardDerivative, Guid> StandardDerivativeService { get; }
        public IAcademicBenchmarkServiceBase<Subject, string> SubjectService { get; }
        public IDbService DbService { get; protected set; }

        private AcademicBenchmarkServiceLocator(UserContext context)
        {
            Context = context;
            
            DbService = new DbService(Settings.AcademicBenchmarkDbConnectionString);
            StandardService = new StandardService(this);

            DocumentService = new AcademicBenchmarkServiceBase<Document, Guid>(this);
            AuthorityService = new AcademicBenchmarkServiceBase<Authority, Guid>(this);
            CourseService = new AcademicBenchmarkServiceBase<Course, Guid>(this);
            GradeLevelService = new AcademicBenchmarkServiceBase<GradeLevel, string>(this);
            StandardDerivativeService = new AcademicBenchmarkServiceBase<StandardDerivative, Guid>(this);
            SubjectService = new AcademicBenchmarkServiceBase<Subject, string>(this);
        }
    }
}
