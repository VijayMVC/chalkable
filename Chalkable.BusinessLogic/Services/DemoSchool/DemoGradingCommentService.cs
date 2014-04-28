using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    //TODO : implementation 
    public class DemoGradingCommentService : DemoSchoolServiceBase, IGradingCommentService
    {
        public DemoGradingCommentService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<GradingComment> gradingComments)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<int> gradingCommentsIds)
        {
            throw new NotImplementedException();
        }

        public IList<GradingComment> GetGradingComments()
        {
            throw new NotImplementedException();
        }
    }
}
