using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingCommentService : BaseDemoIntStorage<GradingComment>, IGradingCommentService
    {
        public DemoGradingCommentService(IServiceLocator serviceLocator, DemoStorage demoStorage) : base(demoStorage, x => x.Id, true)
        {
        }

        public new void Add(IList<GradingComment> gradingComments)
        {
            base.Add(gradingComments);
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            Update(gradingComments);
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public IList<GradingComment> GetGradingComments()
        {
            return data.Select(x => x.Value).ToList();
        }
    }
}
