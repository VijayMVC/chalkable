using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class AssessmentAdminViewData
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
                Role = RoleViewData.Create(CoreRoles.ASSESSMENT_ADMIN_ROLE)
            };
        }
    }
}