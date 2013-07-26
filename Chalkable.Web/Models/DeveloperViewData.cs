using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class DeveloperViewData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string WebSiteLink { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public Guid SchoolId { get; set; }

        private DeveloperViewData() { }

        public static DeveloperViewData Create(Developer developer)
        {
            return new DeveloperViewData
            {
                Id = developer.Id,
                Name = developer.Name,
                WebSiteLink = developer.WebSite,
                Email = developer.Email,
                DisplayName = developer.DisplayName,
                UserId = developer.User.Id,
                SchoolId = developer.SchoolRef
            };
        }
    }
}