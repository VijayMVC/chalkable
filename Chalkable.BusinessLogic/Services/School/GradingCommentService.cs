using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IGradingCommentService
    {
        void Add(IList<GradingComment> gradingComments);
        void Edit(IList<GradingComment> gradingComments);
        void Delete(IList<GradingComment> gradingComments);
        IList<GradingComment> GetGradingComments();
    }

    public class GradingCommentService : SchoolServiceBase, IGradingCommentService
    {
        public GradingCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<GradingComment> gradingComments)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<GradingComment>(u).Insert(gradingComments));
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingComment>(u).Update(gradingComments));
        }

        public void Delete(IList<GradingComment> gradingComments)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradingComment>(u).Delete(gradingComments));
        }

        public IList<GradingComment> GetGradingComments()
        {
            return DoRead(u => new DataAccessBase<GradingComment>(u).GetAll());
        }
    }
}
