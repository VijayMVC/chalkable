using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class GradingCommentViewData
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Comment { get; set; }

        public static GradingCommentViewData Create(GradingComment gradingComment)
        {
            return new GradingCommentViewData
                {
                    Id = gradingComment.Id,
                    Code = gradingComment.Code,
                    Comment = gradingComment.Comment
                };
        }
        public static IList<GradingCommentViewData> Create(IList<GradingComment> gradingComments)
        {
            return gradingComments.Select(Create).ToList();
        } 
    }
}