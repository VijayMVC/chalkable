using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IGradingCommentService
    {
        void Add(IList<GradingComment> gradingComments);
        void Edit(IList<GradingComment> gradingComments);
        void Delete(IList<int> gradingCommentsIds);
        IList<GradingComment> GetGradingComments();
    }

    public class GradingCommentService : SchoolServiceBase, IGradingCommentService
    {
        public GradingCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<GradingComment> gradingComments)
        {
            Modify(da => da.Insert(gradingComments));
        }

        public void Edit(IList<GradingComment> gradingComments)
        {
            Modify(da => da.Update(gradingComments));
        }

        public void Delete(IList<int> gradingCommentsIds)
        {
            Modify(da => da.Delete(gradingCommentsIds));
        }

        private void Modify(Action<GradingCommentDataAccess> modifyMethods)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new GradingCommentDataAccess(uow, Context.SchoolLocalId);
                modifyMethods(da);
                uow.Commit();
            }
        }

        public IList<GradingComment> GetGradingComments()
        {
            using (var uow = Read())
            {
               return new GradingCommentDataAccess(uow, Context.SchoolLocalId).GetAll();
            }
        }
    }
}
