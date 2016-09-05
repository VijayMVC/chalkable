using System;
using SubjectDocument = Chalkable.BusinessLogic.Model.AcademicBenchmark.SubjectDocument;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class SubjectDocumentViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public static SubjectDocumentViewData Create(SubjectDocument subDoc)
        {
            return new SubjectDocumentViewData
            {
                Id = subDoc.Id,
                Description = subDoc.Description
            };
        }

        public static SubjectDocumentViewData Create(Data.AcademicBenchmark.Model.SubjectDoc model)
        {
            return new SubjectDocumentViewData
            {
                Id = model.Id,
                Description = model.Description
            };
        }
    }
}