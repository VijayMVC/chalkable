using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationPersmissionsViewData
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }

        private static IDictionary<AppPermissionType, string> persmissionNameMapper = new Dictionary<AppPermissionType, string>
            {
                {AppPermissionType.User, "User"},
                {AppPermissionType.Class, "Class"},
                {AppPermissionType.Grade, "Grade"},
                {AppPermissionType.Announcement, "Announcement"},
                {AppPermissionType.Attendance, "Attendance"},
                {AppPermissionType.Discipline, "Discipline"},
                {AppPermissionType.Message, "Message"},
                {AppPermissionType.Schedule, "Schedule"},
            }; 

        public static ApplicationPersmissionsViewData Create(ApplicationPermission applicationPermission)
        {
            var res = new ApplicationPersmissionsViewData
            {
                Id = applicationPermission.Id,
                Type = (int)applicationPermission.Permission,
                Name = persmissionNameMapper[applicationPermission.Permission]
            };
            return res;
        }
    public static IList<ApplicationPersmissionsViewData> Create(IList<ApplicationPermission> applicationPermissions)
        {
            return applicationPermissions.Select(Create).ToList();
        }
    }
}