using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private const string CLASS_ID_PARAM = "classId";
        private const string KEYS_LIST_PARAM = "keys";

        public PersonSettingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IList<PersonSetting> GetPersonSettings(IList<string> keys, int? personId, int? schoolYearId, int? classId)
        {
            var conds = new AndQueryCondition();
            if (personId.HasValue)
                conds.Add(nameof(PersonSetting.PersonRef), personId.Value);
            if (schoolYearId.HasValue)
                conds.Add(nameof(PersonSetting.SchoolYearRef), schoolYearId.Value);
            if (classId.HasValue)
                conds.Add(nameof(PersonSetting.ClassRef), classId.Value);
            var q = Orm.SimpleSelect<PersonSetting>(conds);
            if (keys != null && keys.Count > 0)
            {
                var pKyes = new List<string>();
                for (int i = 0; i < keys.Count; i++)
                {
                    var pKey = $"@key_{i + 1}";
                    q.Parameters.Add(pKey, keys[i]);
                    pKyes.Add(pKey);
                }
                q.Sql.Append($" and [{nameof(PersonSetting.Key)}] in ({pKyes.JoinString(",")})");
            }
            return ReadMany<PersonSetting>(q);
        }




        public void AddPersonSettings(IDictionary<string, object> ps, int? personId, int? schoolYearId, int? classId)
        {
            Insert(ps.Select(pair => new PersonSetting
            {
                PersonRef = personId,
                SchoolYearRef = schoolYearId,
                ClassRef = classId,
                Key = pair.Key,
                Value = (pair.Value as DateTime?)?
                    .ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture) 
                    ?? pair.Value?.ToString()
            }).ToList());
        }
    }
}
