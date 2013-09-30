using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDisciplineTypeDataAccess : DataAccessBase<ClassDisciplineType>
    {
        public ClassDisciplineTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void  Delete(Guid classDisciplineId, Guid? disciplineType)
        {
            var condition = new AndQueryCondition {{ClassDisciplineType.CLASS_DISCIPLINE_REF, classDisciplineId}};
            if(disciplineType.HasValue)
                condition.Add(ClassDisciplineType.DISCIPLINE_TYPE_REF, disciplineType);
            SimpleDelete(condition);
        }
    }
}
