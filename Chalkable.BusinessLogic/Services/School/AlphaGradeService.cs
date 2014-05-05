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
        void EditAlphaGrades(IList<AlphaGrade> alphaGrades);
        void Delete(int id);
        void Delete(IList<int> ids);
        IList<AlphaGrade> GetAlphaGrades();

        IList<AlphaGrade> GetAlphaGradesForClass(int classId);
        IList<AlphaGrade> GetAlphaGradesForClassStandards(int classId);
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


        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                new AlphaGradeDataAccess(uow, Context.SchoolLocalId).Delete(ids);
                uow.Commit();
            }
        }


        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new AlphaGradeDataAccess(uow, null).Update(alphaGrades);
                uow.Commit();
            }
        }
        
        public IList<AlphaGrade> GetAlphaGradesForClass(int classId)
        {
            using (var uow = Read())
            {
                return new AlphaGradeDataAccess(uow, Context.SchoolLocalId).GetForClass(classId);
            }
        }

        public IList<AlphaGrade> GetAlphaGradesForClassStandards(int classId)
        {
            using (var uow = Read())
            {
                return new AlphaGradeDataAccess(uow, Context.SchoolLocalId).GetForClassStandards(classId); 
            }
        }
    }
}
