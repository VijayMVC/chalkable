using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class DisciplineTypeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static DisciplineTypeViewData Create(Infraction infraction)
        {
            return new DisciplineTypeViewData
            {
                Id = infraction.InfractionID,
                Name = infraction.Name,
            };
        }
        public static IList<DisciplineTypeViewData> Create(IList<Infraction> infractions)
        {
            return infractions.Select(Create).ToList();
        }
    }
}