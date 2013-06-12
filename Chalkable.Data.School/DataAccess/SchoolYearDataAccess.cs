using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolYearDataAccess : DataAccessBase
    {
        public SchoolYearDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(SchoolYear schoolYear)
        {
            SimpleInsert(schoolYear);
        }
        public void Update(SchoolYear schoolYear)
        {
            SimpleUpdate(schoolYear);
        }
        public void Delete(Guid id)
        {
            SimpleDelete<SchoolYear>(id);
        }
        public SchoolYear GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<SchoolYear>(conds);
        }
        public SchoolYear GetByDate(DateTime date)
        {
            var conds = new Dictionary<string, object> {{"date", date}};
            var sqlCommand = "select * from SchoolYear where StartDate <= @date and EndDate >= @date";
            using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            {
                return reader.ReadOrNull<SchoolYear>();
            }
        }

        public IList<SchoolYear> GetSchoolYears()
        {
            return SelectMany<SchoolYear>();
        }
        public PaginatedList<SchoolYear> GetSchoolYears(int start, int count)
        {
            return PaginatedSelect<SchoolYear>("Id", start, count);
        }

        public bool Exists(string name)
        {
            var conds = new Dictionary<string, object> {{"@name", name}};
            return Exists<SchoolYear>(conds);
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, SchoolYear schoolYear)
        {
            var sqlCommand = "select count(*) as AllCount from SchoolYear where Id != @id and StartDate <= @endDate and EndDate >= @startDate";
            var conds = new Dictionary<string, object>
                {
                    {"id", schoolYear.Id},    
                    {"startDate", startDate},
                    {"endDate", endDate}
                };
            using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            {
                return reader.Read() && SqlTools.ReadInt32(reader, "AllCount") > 0;
            }
        }
    }
}
