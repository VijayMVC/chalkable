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
        IDocumentService DocumentService { get; }
        IAcademicBenchmarkServiceBase<Authority, Guid> AuthorityService { get; }
        ICourseService CourseService { get; }
        IGradeLevelService GradeLevelService { get; }
        IAcademicBenchmarkServiceBase<StandardDerivative, Guid> StandardDerivativeService { get; }
        IAcademicBenchmarkServiceBase<Subject, string> SubjectService { get; }
        ISubjectDocService SubjectDocService { get; }
        ITopicService TopicService { get; }
        ISyncService SyncService { get; }
        IDbService DbService { get; }
    }

    public class AcademicBenchmarkServiceLocator : IAcademicBenchmarkServiceLocator
    {
        public UserContext Context { get; protected set; }
        public IStandardService StandardService { get; }
        public IDocumentService DocumentService { get; }
        public IAcademicBenchmarkServiceBase<Authority, Guid> AuthorityService { get; }
        public ICourseService CourseService { get; }
        public IGradeLevelService GradeLevelService { get; }
        public IAcademicBenchmarkServiceBase<StandardDerivative, Guid> StandardDerivativeService { get; }
        public IAcademicBenchmarkServiceBase<Subject, string> SubjectService { get; }
        public ISubjectDocService SubjectDocService { get; }
        public ITopicService TopicService { get; } 
        public ISyncService SyncService { get; } 
        public IDbService DbService { get; protected set; }

        public AcademicBenchmarkServiceLocator(UserContext context, string connectionString = null)
        {
            Context = context;

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = Settings.AcademicBenchmarkDbConnectionString;

            DbService = new DbService(connectionString);
            StandardService = new StandardService(this);

            DocumentService = new DocumentService(this);
            AuthorityService = new AcademicBenchmarkServiceBase<Authority, Guid>(this);
            CourseService = new CourseService(this);
            GradeLevelService = new GradeLevelService(this);
            StandardDerivativeService = new AcademicBenchmarkServiceBase<StandardDerivative, Guid>(this);
            SubjectService = new AcademicBenchmarkServiceBase<Subject, string>(this);
            SubjectDocService = new SubjectDocService(this);
            TopicService = new TopicService(this);
            SyncService = new SyncService(this);
        }
    }
}
