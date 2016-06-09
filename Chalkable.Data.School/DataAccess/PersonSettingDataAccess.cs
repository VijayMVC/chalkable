using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonSettingDataAccess : DataAccessBase<PersonSetting, int>
    {
        private const string SP_GET_PERSON_SETTINGS = "spGetPersonSettings";
        private const string PERSON_ID_PARAM = "personId";
        private const string SCHOOL_YEAR_ID_PARAM = "schoolYearId";
        private const string KEYS_LIST_PARAM = "keys";

        public PersonSettingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IDictionary<string, string> GetPersonSettings(int personId, int schoolYearId, IList<string> keys)
        {
            var param = new Dictionary<string, object>
            {
                {PERSON_ID_PARAM, personId},
                {SCHOOL_YEAR_ID_PARAM, schoolYearId },
                {KEYS_LIST_PARAM, keys}
            };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_PERSON_SETTINGS, param))
            {
                return reader.ReadList<PersonSetting>().ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public void AddPersonSettings(int personId, int schoolYearId, IDictionary<string, object> ps)
        {
            Insert(ps.Select(pair => new PersonSetting
            {
                PersonRef = personId,
                SchoolYearRef = schoolYearId,
                Key = pair.Key,
                Value = (pair.Value as DateTime?)?
                    .ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture) 
                    ?? pair.Value?.ToString()
            }).ToList());
        }
    }
}
