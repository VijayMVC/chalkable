using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class StandardViewData
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public AuthorityViewData Authority { get; set; }
        public DocumentViewData Document { get; set; }

        public static StandardViewData Create(Standard standard)
        {
            return new StandardViewData
            {
                Id = standard.Id,
                Code = standard.Code,
                Description = standard.Description,
                IsDeepest = standard.IsDeepest,
                ParentId = standard.ParentId,
                Level = standard.Level,
                IsActive = standard.IsActive,
                Authority = AuthorityViewData.Create(standard.Authority),
                Document = DocumentViewData.Create(standard.Document)
            };
        }

        public static StandardViewData Create(StandardInfo model)
        {
            return new StandardViewData
            {
                Id = model.Id,
                Description = model.Description,
                Code = model.Code,
                IsActive = model.IsActive,
                IsDeepest = model.IsDeepest,
                Level = model.Level,
                ParentId = model.ParentId,
                Authority = AuthorityViewData.Create(model.Authority),
                Document = DocumentViewData.Create(model.Document)
            };
        }
    }
}