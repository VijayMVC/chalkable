using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class IdNameViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static IdNameViewData Create(Guid id, string name)
        {
            return new IdNameViewData
                {
                    Id = id,
                    Name = name
                };
        }
    }
}