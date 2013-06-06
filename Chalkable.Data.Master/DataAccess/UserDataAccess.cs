using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class UserDataAccess : DataAccessBase
    {
        public UserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private static string BuildSql(Dictionary<string, object> conditions)
        {
            var p1 = new StringBuilder();
            var p2 = new StringBuilder();
            p1.Append("select * from [User]");
            p2.Append(@"select * from
                            [User]
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
                    var sul = reader.ReadList<SchoolUser>();
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
                var sul = reader.ReadList<SchoolUser>();
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
            var sql = @"Insert into [User] (Id, Login, Password, IsSysAdmin, IsDeveloper) values (@id, @login, @password, @issysadmin, @isdeveloper)";
            ExecuteNonQueryParametrized(sql, new Dictionary<string, object>
                {
                    {"@id", user.Id},
                    {"@login", user.Login},
                    {"@password", user.Password},
                    {"@issysadmin", user.IsSysAdmin},
                    {"@isdeveloper", user.IsDeveloper},
                });
        }
    }
}
