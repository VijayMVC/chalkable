using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILimitedEnglishService
    {
        void Add(IList<LimitedEnglish> models);
        void Edit(IList<LimitedEnglish> models);
        void Delete(IList<LimitedEnglish> models);
    }

    public class LimitedEnglishService : SchoolServiceBase, ILimitedEnglishService
    {
        public LimitedEnglishService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<LimitedEnglish> models)
        {
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Insert(models));
        }

        public void Edit(IList<LimitedEnglish> models)
        {
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Update(models));
        }

        public void Delete(IList<LimitedEnglish> models)
        {
            DoUpdate(u => new DataAccessBase<LimitedEnglish>(u).Delete(models));
        }
    }
}
