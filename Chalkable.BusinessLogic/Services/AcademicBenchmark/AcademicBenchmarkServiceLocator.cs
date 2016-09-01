using System;
using System.Configuration;
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
        IAcademicBenchmarkServiceBase<SubjectDoc, Guid> SubjectDocService { get; }
        IAcademicBenchmarkServiceBase<Topic, Guid> TopicService { get; }
        ISyncService SyncService { get; }
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
        public IAcademicBenchmarkServiceBase<SubjectDoc, Guid> SubjectDocService { get; }
        public IAcademicBenchmarkServiceBase<Topic, Guid> TopicService { get; } 
        public ISyncService SyncService { get; } 
        public IDbService DbService { get; protected set; }

        public AcademicBenchmarkServiceLocator(UserContext context, string connectionString = null)
        {
            Context = context;

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = Settings.AcademicBenchmarkDbConnectionString;

            DbService = new DbService(connectionString);
            StandardService = new StandardService(this);

            DocumentService = new AcademicBenchmarkServiceBase<Document, Guid>(this);
            AuthorityService = new AcademicBenchmarkServiceBase<Authority, Guid>(this);
            CourseService = new AcademicBenchmarkServiceBase<Course, Guid>(this);
            GradeLevelService = new AcademicBenchmarkServiceBase<GradeLevel, string>(this);
            StandardDerivativeService = new AcademicBenchmarkServiceBase<StandardDerivative, Guid>(this);
            SubjectService = new AcademicBenchmarkServiceBase<Subject, string>(this);
            SubjectDocService = new AcademicBenchmarkServiceBase<SubjectDoc, Guid>(this);
            TopicService = new AcademicBenchmarkServiceBase<Topic, Guid>(this);
            SyncService = new SyncService(this);
        }
    }
}
