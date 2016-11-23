using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IMealTypeService
    {
        void Add(IList<MealType> mealTypes);
        void Edit(IList<MealType> mealTypes);
        void Delete(IList<MealType> mealTypes);
        IList<MealType> GetAll();
    }

    public class MealTypeService : SchoolServiceBase, IMealTypeService
    {
        public MealTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<MealType> mealTypes)
        {
            DoUpdate(u => new DataAccessBase<MealType>(u).Insert(mealTypes));
        }

        public void Edit(IList<MealType> mealTypes)
        {
            DoUpdate(u => new DataAccessBase<MealType>(u).Update(mealTypes));
        }

        public void Delete(IList<MealType> mealTypes)
        {
            DoUpdate(u => new DataAccessBase<MealType>(u).Delete(mealTypes));
        }

        public IList<MealType> GetAll()
        {
            var conds = new AndQueryCondition { { nameof(MealType.IsActive), true } };
            return DoRead(u => new DataAccessBase<MealType>(u).GetAll(conds)).OrderBy(x => x.Name).ToList();
        }
    }
}
