using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILimitedEnglishService
    {
        void Add(IList<LimitedEnglish> models);
        void Edit(IList<LimitedEnglish> models);
        void Delete(IList<LimitedEnglish> models);
        IList<LimitedEnglish> GetList(bool? activeOnly);
    }

    public class LimitedEnglishService : SchoolServiceBase, ILimitedEnglishService
    {
        public LimitedEnglishService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<LimitedEnglish> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Insert(models));
        }

        public void Edit(IList<LimitedEnglish> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Update(models));
        }

        public void Delete(IList<LimitedEnglish> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Delete(models));
        }

        public IList<LimitedEnglish> GetList(bool? activeOnly)
        {
            var conds = new AndQueryCondition();
            if(activeOnly.HasValue)
                conds.Add(nameof(LimitedEnglish.IsActive), activeOnly.Value);
            return DoRead(u => new DataAccessBase<LimitedEnglish>(u).GetAll());
        }
    }
}
