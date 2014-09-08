using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class UserDataAccess : DataAccessBase<User, Guid>
    {
        public UserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private static DbQuery BuildSql(QueryCondition conditions)
        {
            var userQuery = new DbQuery();
            var suQuery = new DbQuery();
            var typesL1 = new List<Type> { typeof(User), typeof(District) };
            var typesL2 = new List<Type> { typesL1[0], typeof(SchoolUser), typeof(School) };
            userQuery.Sql.AppendFormat(@"select {0} from [{1}] left join [{2}] on [{1}].{3} = [{2}].{4}"
                                       , Orm.ComplexResultSetQuery(typesL1), typesL1[0].Name, typesL1[1].Name,
                                       User.DISTRICT_REF_FIELD, District.ID_FIELD);
            conditions.BuildSqlWhere(userQuery, typesL1[0].Name);

            suQuery.Sql.AppendFormat(@"select {0} from [User]
                                       join SchoolUser on [User].Id = SchoolUser.UserRef
                                       join School on SchoolUser.SchoolRef = School.Id", Orm.ComplexResultSetQuery(typesL2));
            conditions.BuildSqlWhere(suQuery, typesL1[0].Name);
            return new DbQuery(new List<DbQuery> { userQuery, suQuery });
        } 


        public void Delete(IList<int> localIds, Guid districtId)
        {
            if (localIds != null && localIds.Count > 0)
            {
                var builder = new StringBuilder();
                var whereScrip = string.Format(" [{0}].[{1}] = @[{1}] and [{0}].[{2}] in ({3})"
                                               , "User", User.DISTRICT_REF_FIELD, User.LOCAL_ID_FIELD,
                                               localIds.Select(x => x.ToString()).JoinString(","));
                builder.AppendFormat("delete from SchoolUser where [{0}] in (select [Id] from [User] where {1}) "
                                     , SchoolUser.USER_REF_FIELD, whereScrip)
                       .AppendFormat("delete from [User] where {0}", whereScrip);
                var parameters = new Dictionary<string, object>{{User.DISTRICT_REF_FIELD, districtId}};
                ExecuteNonQueryParametrized(builder.ToString(), parameters);        
            }
        }

        public User GetUser(QueryCondition conditions)
        {
            var dbQuery = BuildSql(conditions);
            return Read(dbQuery, ReadGetUserQueryResult);
        }
        private User ReadGetUserQueryResult(DbDataReader reader)
        {
            User res = null;
            if (reader.Read())
            {
                res = reader.Read<User>(true);
                if (res.DistrictRef.HasValue)
                    res.District = reader.Read<District>(true);
                reader.NextResult();
                var sul = reader.ReadList<SchoolUser>(true);
                foreach (var schoolUser in sul)
                {
                    schoolUser.User = res;
                    schoolUser.School.District = res.District;
                }
                res.SchoolUsers = sul;
            }
            return res;
        }

        public User GetSysAdmin()
        {
            var conds = new AndQueryCondition{{User.IS_SYS_ADMIN_FIELD, true}};
            return GetUser(conds);
        }
        public User GetUser(string login, string password, Guid? id)
        {
            var conds = new AndQueryCondition();
            if (login != null)
                conds.Add(User.LOGIN_FIELD, login);
            if (password != null)
                conds.Add(User.PASSWORD_FIELD, password);
            if (id != null)
                conds.Add(User.ID_FIELD, id);
            return GetUser(conds);
        }

        public User GetUser(string confirmationKey)
        {
            var conds = new AndQueryCondition();
            if(!string.IsNullOrEmpty(confirmationKey))
                conds.Add(User.CONFIRMATION_KEY_FIELD, confirmationKey);
            return GetUser(conds);
        }

        public IList<User> GetUsers()
        {
            return GetUsers(new AndQueryCondition());
        }
        public IList<User> GetUsers(Guid districtId)
        {
            return GetUsers(new AndQueryCondition { { User.DISTRICT_REF_FIELD, districtId } });
        } 
        private IList<User> GetUsers(QueryCondition conds)
        {
            var dbQeury = BuildSql(conds);
            return Read(dbQeury, ReadGetUsersQueryResult);
        }
        private IList<User> ReadGetUsersQueryResult(DbDataReader reader)
        {
            var res = new List<User>();
            while (reader.Read())
            {
                var u = reader.Read<User>(true);
                if (u.DistrictRef.HasValue)
                    u.District = reader.Read<District>(true);
                res.Add(u);
            }
            reader.NextResult();
            var sul = reader.ReadList<SchoolUser>(true);
            var sulDic = res.ToDictionary(x => x.Id, x => new List<SchoolUser>());
            foreach (var schoolUser in sul)
            {
                sulDic[schoolUser.UserRef].Add(schoolUser);
            }
            foreach (var user in res)
            {
                user.SchoolUsers = sulDic[user.Id];
            }
            return res;
        }

        public void UpdateSisUserNames(List<Pair<string, string>> values)
        {
            if (2 * values.Count > MAX_PARAMETER_NUMBER)
            {
                var list1 = values.Take(values.Count / 2).ToList();
                UpdateSisUserNames(list1);
                UpdateSisUserNames(values.Skip(list1.Count).ToList());
            }
            else
            {
                if (values.Count > 0)
                {
                    var t = typeof(User);
                    var b = new StringBuilder();
                    IDictionary<string, object> ps = new Dictionary<string, object>();
                    for (int i = 0; i < values.Count; i++)
                    {

                        var sql = string.Format("Update [{0}] set SisUserName = @new_{1} where SisUserName = @old_{1}", t.Name, i);
                        b.Append(sql).Append(" ");
                        ps.Add("new_" + i, values[i].Second);
                        ps.Add("old_" + i, values[i].First);
                    }
                    ExecuteNonQueryParametrized(b.ToString(), ps, 10 + values.Count);    
                }
            }
        }
    }
}
