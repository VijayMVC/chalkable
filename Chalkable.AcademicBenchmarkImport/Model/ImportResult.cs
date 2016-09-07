using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.AcademicBenchmarkImport.Mappers;

namespace Chalkable.AcademicBenchmarkImport.Model
{

    public class ResultBase
    {
        public ResultBase()
        {
            Authorities = new List<Authority>();
            Courses = new List<Course>();
            Documents = new List<Document>();
            GradeLevels =new List<GradeLevel>();
            SubjectDocuments = new List<SubjectDocument>();
            Subjects = new List<Subject>();
        }

        public ResultBase(ResultBase model)
        {
            Authorities = model.Authorities;
            Courses = model.Courses;
            Documents = model.Documents;
            GradeLevels = model.GradeLevels;
            SubjectDocuments = model.SubjectDocuments;
            Subjects = model.Subjects;
        }

        public IList<Authority> Authorities { get; set; }
        public IList<Course> Courses { get; set; }
        public IList<Document> Documents { get; set; }
        public IList<GradeLevel> GradeLevels { get; set; }
        public IList<Subject> Subjects { get; set; }
        public IList<SubjectDocument> SubjectDocuments { get; set; }
        public IList<Standard> Standards { get; set; }
        public IList<Topic> Topics { get; set; }
    }

    public class ImportResult : ResultBase
    {
        public ImportResult()
        { }

        public ImportResult(ResultBase model) : base(model)
        { }
    }

    public class SyncResult : ResultBase
    {
        public IList<SyncItem> StandardSyncItems { get; set; }
        public IList<SyncItem> TopicSyncItems { get; set; }

        public SyncResult()
        {
            StandardSyncItems = new List<SyncItem>();
            TopicSyncItems = new List<SyncItem>();
        }

        public SyncResult(ResultBase model) : base(model)
        {
            StandardSyncItems = new List<SyncItem>();
            TopicSyncItems = new List<SyncItem>();
        }
    }

    internal class SyncData<T, TId>
    {
        public IList<T> Update { get; set; }
        public IList<T> Insert { get; set; }
        public IList<TId> Delete { get; set; }

        public static SyncData<Standard, Guid> Create(IList<SyncItem> syncItems, IList<Standard> models)
        {
            var forUpdate = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Update);
            var forInsert = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Insert);
            var forDelete = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Delete);

            return new SyncData<Standard, Guid>
            {
                Insert = models?.Where(x => forInsert.Any(y => y.Id == x.Id)).ToList(),
                Update = models?.Where(x => forUpdate.Any(y => y.Id == x.Id)).ToList(),
                Delete = forDelete.Select(x => x.Id).ToList()
            };
        }

        public static SyncData<Topic, Guid> Create(IList<SyncItem> syncItems, IList<Topic> topics)
        {
            var forUpdate = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Update);
            var forInsert = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Insert);
            var forDelete = syncItems.Where(x => MapperHelper.Map(x.ChangeType) == OperationType.Delete);

            return new SyncData<Topic, Guid>
            {
                Insert = topics?.Where(x => forInsert.Any(y => y.Id == x.Id)).ToList(),
                Update = topics?.Where(x => forUpdate.Any(y => y.Id == x.Id)).ToList(),
                Delete = forDelete.Select(x => x.Id).ToList()
            };
        }
    }
}
