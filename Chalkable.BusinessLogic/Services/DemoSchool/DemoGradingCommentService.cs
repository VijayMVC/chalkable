using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingCommentStorage : BaseDemoIntStorage<GradingComment>
    {
        public DemoGradingCommentStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoGradingCommentService : DemoSchoolServiceBase, IGradingCommentService
    {
        private DemoGradingCommentStorage GradingCommentStorage { get; set; }
        public DemoGradingCommentService(IServiceLocatorSchool schoolLocator) 
            : base(schoolLocator)
        {
            GradingCommentStorage = new DemoGradingCommentStorage();
        }

        public void Add(IList<GradingComment> gradingComments)
        {
            GradingCommentStorage.Add(gradingComments);
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            GradingCommentStorage.Update(gradingComments);
        }

        public void Delete(IList<GradingComment> gradingComments)
        {
            GradingCommentStorage.Delete(gradingComments);
        }

        public IList<GradingComment> GetGradingComments()
        {
            return GradingCommentStorage.GetAll();
        }
    }
}
