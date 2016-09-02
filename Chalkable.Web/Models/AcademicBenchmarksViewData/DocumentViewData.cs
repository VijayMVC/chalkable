using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class DocumentViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static DocumentViewData Create(Document document)
        {
            return new DocumentViewData
            {
                Id = document.Id,
                Description = document.Description,
                Code = document.Code
            };
        }

        public static DocumentViewData Create(Data.AcademicBenchmark.Model.Document model)
        {
            return new DocumentViewData
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Title
            };
        }
    }
}