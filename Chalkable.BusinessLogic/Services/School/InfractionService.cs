using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IInfractionService
    {
        void Add(IList<Infraction> infractions);
        void Edit(IList<Infraction> infractions);
        void Delete(IList<Infraction> infractions);
        IList<Infraction> GetInfractions();
    }

    public class InfractionService : SchoolServiceBase, IInfractionService
    {
        public InfractionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Infraction> infractions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Infraction>(u).Insert(infractions));
        }

        public void Edit(IList<Infraction> infractions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Infraction>(u).Update(infractions));
        }

        public void Delete(IList<Infraction> infractions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<Infraction>(u).Delete(infractions));
        }

        public IList<Infraction> GetInfractions()
        {
            return DoRead(u => new DataAccessBase<Infraction>(u).GetAll());
        }
    }
}
