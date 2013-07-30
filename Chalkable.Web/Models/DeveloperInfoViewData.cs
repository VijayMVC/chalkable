using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class DeveloperInfoViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public Guid SchoolId { get; set; }

        public static DeveloperInfoViewData Create(Developer developer)
        {
            return new DeveloperInfoViewData
                {
                    Id = developer.Id,
                    DisplayName = developer.DisplayName,
                    Name = developer.Name,
                    Email = developer.Email,
                    WebSite = developer.WebSite,
                    SchoolId = developer.SchoolRef
                };
        }
    }
}