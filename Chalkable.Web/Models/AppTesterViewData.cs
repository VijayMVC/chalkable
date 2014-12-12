using System;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AppTesterViewData
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public RoleViewData Role { get; set; }

        public static AppTesterViewData Create(User user)
        {
            return new AppTesterViewData
                {
                    Id = user.Id,
                    DisplayName = user.Login,
                    Email = user.Login,
                    Role = RoleViewData.Create(CoreRoles.APP_TESTER_ROLE)
                };
        }
    }
}