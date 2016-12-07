using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolProgramDataAccess : DataAccessBase<SchoolProgram, int>
    {
        public SchoolProgramDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<SchoolProgram> GetAll()
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append($"SELECT * FROM [{nameof(SchoolProgram)}] WHERE [{nameof(SchoolProgram.IsActive)}] = 1 ORDER BY [{nameof(SchoolProgram.Name)}]");
            return ReadMany<SchoolProgram>(dbQuery);
        } 
    }
}
