using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonSettingDataAccess : DataAccessBase<PersonSetting, int>
    {
        private const string SP_GET_PERSON_SETTINGS = "spGetPersonSettings";
        private const string PERSON_ID_PARAM = "personId";
        private const string KEYS_LIST_PARAM = "keys";

        public PersonSettingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<PersonSetting> GetPersonSettings(int personId, IList<string> keys)
        {
            var param = new Dictionary<string, object>
            {
                {PERSON_ID_PARAM, personId},
                {KEYS_LIST_PARAM, keys}
            };
            using (var reader = ExecuteStoredProcedureReader(SP_GET_PERSON_SETTINGS, param))
            {
                return reader.ReadList<PersonSetting>();
            }
        }

        public void AddPersonSettings(int personId, IDictionary<string, object> ps)
        {
            var personSettings =
                ps.Select(
                    pair => new PersonSetting() {PersonRef = personId, Key = pair.Key, Value = pair.Value.ToString()});
            Insert(personSettings.ToList());
        }
    }
}
