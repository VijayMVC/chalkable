using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class UserDataAccess : DataAccessBase<User>
    {
        public UserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private static string BuildSql(Dictionary<string, object> conditions)
        {
            var p1 = new StringBuilder();
            var p2 = new StringBuilder();
            p1.Append("select * from [User]");
            p2.Append(@"select 
                              [User].Id as User_Id,
                              [User].Login as User_Login,
                              [User].Password as User_Password,
                              [User].IsSysAdmin as User_IsSysAdmin,
                              [User].IsDeveloper as User_IsDeveloper,
                              SchoolUser.Id as SchoolUser_Id,
                              SchoolUser.UserRef as SchoolUser_UserRef,
                              SchoolUser.SchoolRef as SchoolUser_SchoolRef,
                              SchoolUser.Role as SchoolUser_Role,
                              School.Id as School_Id,
                              School.Name as School_Name,
                              School.ServerUrl as School_ServerUrl,
                              School.DistrictRef as School_DistrictRef,
                              School.IsEmpty as School_IsEmpty
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
                var res = reader.ReadOrNull<User>();
                if (res != null)
                {
                    reader.NextResult();
                    var sul = reader.ReadList<SchoolUser>(true);
                    res.SchoolUsers = sul;
                }
                return res;
            }
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

        public IList<User> GetUsers()
        {
            var conds = new Dictionary<string, object>();
            var sql = BuildSql(conds);
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                var res = reader.ReadList<User>();
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
