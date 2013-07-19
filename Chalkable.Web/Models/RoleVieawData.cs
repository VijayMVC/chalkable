using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;

namespace Chalkable.Web.Models
{
    public class RoleVieawData 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LowerredName { get; set; }
        public string Description { get; set; }

        public static RoleVieawData Create(CoreRole role)
        {
            return new RoleVieawData
                {
                    Id = role.Id,
                    Name = role.Name,
                    LowerredName = role.LoweredName,
                    Description = role.Description
                };
        } 
        public static IList<RoleVieawData> Create(IList<CoreRole> roles)
        {
            return roles.Select(Create).ToList();
        } 
    }
}