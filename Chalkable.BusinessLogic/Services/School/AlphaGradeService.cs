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
    public interface IAlphaGradeService
    {
        void AddAlphaGrades(IList<AlphaGrade> alphaGrades);
        void Delete(int id);
        IList<AlphaGrade> GetAlphaGrades();
    }

    public class AlphaGradeService : SchoolServiceBase, IAlphaGradeService
    {
        public AlphaGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                new AlphaGradeDataAccess(uow, null).Insert(alphaGrades);
                uow.Commit();
            }
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new AlphaGradeDataAccess(uow, null).Delete(id);
                uow.Commit();
            }
        }

        public IList<AlphaGrade> GetAlphaGrades()
        {
            using (var uow = Read())
            {
                return new AlphaGradeDataAccess(uow, Context.SchoolLocalId).GetAll();
            }
        }
    }
}
