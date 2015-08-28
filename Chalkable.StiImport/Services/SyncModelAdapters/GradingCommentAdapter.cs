using System.Collections.Generic;
using System.Linq;
using GradingComment = Chalkable.StiConnector.SyncModel.GradingComment;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradingCommentAdapter : SyncModelAdapter<GradingComment>
    {
        public GradingCommentAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<GradingComment> entities)
        {
            var gc = entities.Select(x => new Data.School.Model.GradingComment
            {
                Code = x.Code,
                Comment = x.Comment,
                Id = x.GradingCommentID,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingCommentService.Add(gc);
        }

        protected override void UpdateInternal(IList<GradingComment> entities)
        {
            var gc = entities.Select(x => new Data.School.Model.GradingComment
            {
                Code = x.Code,
                Comment = x.Comment,
                Id = x.GradingCommentID,
                SchoolRef = x.SchoolID
            }).ToList();
            ServiceLocatorSchool.GradingCommentService.Edit(gc);
        }

        protected override void DeleteInternal(IList<GradingComment> entities)
        {
            var gradingComments = entities.Select(x => new Data.School.Model.GradingComment { Id = x.GradingCommentID }).ToList();
            ServiceLocatorSchool.GradingCommentService.Delete(gradingComments);
        }
    }
}