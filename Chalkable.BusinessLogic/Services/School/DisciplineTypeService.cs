using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDisciplineTypeService
    {
        DisciplineType Add(string name, int score);
        DisciplineType Edit(Guid id, string name, int score);
        void Delete(Guid id);
        PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count);
        DisciplineType GetDisciplineTypeById(Guid id);
    }

    public class DisciplineTypeService : SchoolServiceBase, IDisciplineTypeService
    {
        public DisciplineTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public DisciplineType Add(string name, int score)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var res = new DisciplineType
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Score = score
                    };
                new DisciplineTypeDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }

        public DisciplineType Edit(Guid id, string name, int score)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new DisciplineTypeDataAccess(uow);
                var res = da.GetById(id);
                res.Name = name;
                res.Score = score;
                da.Update(res);
                uow.Commit();
                return res;
            }
        }
        public void Delete(Guid id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DisciplineTypeDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count)
        {
            using (var uow = Read())
            {
                return new DisciplineTypeDataAccess(uow).GetPage(start, count, DisciplineType.SCORE_FIELD);
            }
        }

        public DisciplineType GetDisciplineTypeById(Guid id)
        {
            using (var uow = Read())
            {
                return new DisciplineTypeDataAccess(uow).GetById(id);
            }
        }
    }
}
