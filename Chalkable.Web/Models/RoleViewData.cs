using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;

namespace Chalkable.Web.Models
{
    public class RoleViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NameLowered { get; set; }
        private RoleViewData()
        {
        }

        public static RoleViewData Create(CoreRole role)
        {
            var res = new RoleViewData
                          {
                              Id = role.Id,
                              Name = role.Name,
                              NameLowered = role.LoweredName,
                              Description = role.Description
                          };

            return res;
        }
        public static IList<RoleViewData> Create(IList<CoreRole> roles)
        {
            var res = roles.Select(Create);
            return res.ToList();
        }
    }
}