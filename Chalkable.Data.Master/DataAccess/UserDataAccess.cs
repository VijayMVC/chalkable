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
            var typesL1 = new List<Type> { typeof(User), typeof(District), typeof(UserLoginInfo) };
            var typesL2 = new List<Type> { typesL1[0], typeof(SchoolUser), typeof(School) };
            userQuery.Sql.AppendFormat(@"select {0} from [{1}] 
                                         join [{2}] on [{1}].{3} = [{2}].{4}
                                         left join [{5}] on [{5}].[{6}] = [{1}].[{7}]"
                                       , Orm.ComplexResultSetQuery(typesL1), typesL1[0].Name, typesL1[1].Name,
                                       User.DISTRICT_REF_FIELD, District.ID_FIELD, typesL1[2].Name, UserLoginInfo.ID_FIELD, User.ID_FIELD);
            conditions.BuildSqlWhere(userQuery, typesL1[0].Name);

            suQuery.Sql.AppendFormat(@"select {0} from [User]
                                       join SchoolUser on [User].[{1}] = SchoolUser.[{2}]
                                       join School on SchoolUser.[{3}] = School.[{4}]
                                       left join UserLoginInfo on UserLoginInfo.[{5}] = [User].[{6}]", Orm.ComplexResultSetQuery(typesL2)
                                     , User.SIS_USER_ID_FIELD, SchoolUser.USER_REF_FIELD, SchoolUser.SCHOOL_REF_FIELD, School.LOCAL_ID_FIELD
                                     , UserLoginInfo.ID_FIELD, User.ID_FIELD);
            conditions.BuildSqlWhere(suQuery, typesL1[0].Name);
            return new DbQuery(new List<DbQuery> { userQuery, suQuery });
        } 

        public void UpdateUsersForImport(IList<User> users)
        {
            var table = typeof (User).Name;
            if (users.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var user in users)
                {
                    builder.AppendFormat("Update [{0}] set [{1}]='{2}', [{3}]='{4}' where [{5}]='{6}' and {7}={8} ",
                                         table, User.SIS_USER_NAME_FIELD, user.SisUserName
                                         , User.FULL_NAME_FIELD, user.FullName, User.DISTRICT_REF_FIELD,
                                         user.DistrictRef, User.SIS_USER_ID_FIELD, user.SisUserId);
                }
                ExecuteNonQueryParametrized(builder.ToString(), new Dictionary<string, object>(), 10+users.Count);
            }
            
        }

        public void Delete(IList<int> sisUserIds, Guid districtId)
        {
            if (sisUserIds != null && sisUserIds.Count > 0)
            {
                var builder = new StringBuilder();
                var whereScrip = string.Format(" [{0}].[{1}] = @[{1}] and [{0}].[{2}] in ({3})"
                                               , "User", User.DISTRICT_REF_FIELD, User.SIS_USER_ID_FIELD,
                                               sisUserIds.Select(x => x.ToString()).JoinString(","));
                       builder.AppendFormat("delete from UserLoginInfo where [{0}] in (select [Id] from [User] where {1}) "
                                     , UserLoginInfo.ID_FIELD, whereScrip)
                       .AppendFormat("delete from [User] where {0} ", whereScrip);
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
                var loginInfoId = SqlTools.ReadGuidNull(reader, string.Format("{0}_{1}", typeof(UserLoginInfo).Name, UserLoginInfo.ID_FIELD));
                if(loginInfoId != null)
                    res.LoginInfo = reader.Read<UserLoginInfo>(true);
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
    }
}
