using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IInfractionService
    {
        void Add(IList<Infraction> infractions);
        void Edit(IList<Infraction> infractions);
        void Delete(IList<int> ids);
        IList<Infraction> GetInfractions(bool onlyActive = true);
    }

    public class InfractionService : SchoolServiceBase, IInfractionService
    {
        public InfractionService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Infraction> infractions)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new InfractionDataAccess(uow).Insert(infractions);
                uow.Commit();
            }
        }

        public void Edit(IList<Infraction> infractions)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new InfractionDataAccess(uow).Update(infractions);
                uow.Commit();
            }
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new InfractionDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }

        public IList<Infraction> GetInfractions(bool onlyActive = true)
        {
            using (var uow = Read())
            {
                var conds = new AndQueryCondition();
                if(onlyActive)
                    conds.Add(Infraction.IS_ACTIVE_FIELD, 1, ConditionRelation.Equal);
                return new InfractionDataAccess(uow).GetAll(conds);
            }
        }
    }
}
