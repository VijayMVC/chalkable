using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class SysAdminViewData
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public RoleViewData Role { get; set; }

        public static SysAdminViewData Create(User user)
        {
            return new SysAdminViewData
                {
                    Id = user.Id,
                    DisplayName = user.Login,
                    Email = user.Login,
                    Role = RoleViewData.Create(CoreRoles.SUPER_ADMIN_ROLE)
                };
        }
    }
}