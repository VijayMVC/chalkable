using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class UserDataAccess : DataAccessBase<User, Guid>
    {
        public UserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private static string BuildSql(Dictionary<string, object> conditions)
        {
            var p1 = new StringBuilder();
            var p2 = new StringBuilder();
            p1.Append(@"select 
                    [User].Id as [User_Id],
                    [User].LocalId as User_LocalId,
                    [User].Login as User_Login,
                    [User].Password as User_Password,
                    [User].IsSysAdmin as User_IsSysAdmin,
                    [User].IsDeveloper as User_IsDeveloper,
                    [User].ConfirmationKey as User_ConfirmationKey,
                    [User].Districtref as User_DistrictRef,
                    District.Id as District_Id,
                    District.Name as District_Name,
                    District.SisUrl as District_SisUrl,
                    District.DbName as District_DbName,
                    District.SisUserName as District_SisUserName,
                    District.SisPassword as District_SisPassword,
                    District.Status as District_Status,
                    District.TimeZone as District_TimeZone,
                    District.DemoPrefix as District_DemoPrefix,
                    District.LastUseDemo as District_LastUseDemo,
                    District.ServerUrl as District_ServerUrl,
                    District.IsEmpty as District_IsEmpty
                    from 
                    [User]
                    left join District on [User].DistrictRef = District.Id");
            p2.Append(@"select 
                              [User].Id as User_Id,
                              [User].Login as User_Login,
                              [User].Password as User_Password,
                              [User].IsSysAdmin as User_IsSysAdmin,
                              [User].IsDeveloper as User_IsDeveloper,
                              [User].ConfirmationKey as User_ConfirmationKey,
                              SchoolUser.Id as SchoolUser_Id,
                              SchoolUser.UserRef as SchoolUser_UserRef,
                              SchoolUser.SchoolRef as SchoolUser_SchoolRef,
                              SchoolUser.Role as SchoolUser_Role,
                              School.Id as School_Id,
                              School.Name as School_Name,
                              School.DistrictRef as School_DistrictRef                              
                        from [User]
                        join SchoolUser on [User].Id = SchoolUser.UserRef
                        join School on SchoolUser.SchoolRef = School.Id");
            if (conditions.Count > 0)
            {
                p1.Append(" where ");
                p2.Append(" where ");
            }
            bool first = true;
            foreach (var condition in conditions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    p1.Append(" and ");
                    p2.Append(" and ");
                }
                p1.Append("[User].").Append(condition.Key).Append("=").Append("@").Append(condition.Key);
                p2.Append("[User].").Append(condition.Key).Append("=").Append("@").Append(condition.Key);
            }

            var sql = p1.Append(";").Append(p2).ToString();
            return sql;
        }

        public User GetUser(Dictionary<string, object> conditions)
        {
            var sql = BuildSql(conditions);
            using (var reader = ExecuteReaderParametrized(sql, conditions))
            {
                User res = null;
                if (reader.Read())
                {
                    res = reader.Read<User>(true);
                    if (res.DistrictRef.HasValue)
                        res.District = reader.Read<District>(true);
                    reader.NextResult();
                    var sul = reader.ReadList<SchoolUser>(true);
                    res.SchoolUsers = sul;
                }
                return res;
            }
        }

        public User GetSysAdmin()
        {
            var conds = new Dictionary<string, object> {{"IsSysAdmin", true}};
            return GetUser(conds);
        }

        public User GetUser(string login, string password, Guid? id)
        {
            var conds = new Dictionary<string, object>();
            if (login != null)
                conds.Add("login", login);
            if (password != null)
                conds.Add("password", password);
            if (id != null)
                conds.Add("id", id);
            return GetUser(conds);
        }

        public User GetUser(string confirmationKey)
        {
            var conds = new Dictionary<string, object>();
            if(string.IsNullOrEmpty(confirmationKey))
                conds.Add(User.CONFIRMATION_KEY_FIELD, confirmationKey);
            return GetUser(conds);
        }

        public IList<User> GetUsers()
        {
            var conds = new Dictionary<string, object>();
            var sql = BuildSql(conds);
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                var res = new List<User>();
                while (reader.Read())
                {
                    var u = reader.Read<User>(true);
                    if (u.DistrictRef.HasValue)
                        u.District = reader.Read<District>(true);
                }
                reader.NextResult();
                var sul = reader.ReadList<SchoolUser>(true);
                var sulDic = res.ToDictionary(x => x.Id, x=>new List<SchoolUser>());
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
        }

        public void Create(User user)
        {
            SimpleInsert(user);
        }
    }
}
