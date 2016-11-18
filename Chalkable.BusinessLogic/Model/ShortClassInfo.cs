using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ShortClassInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static ShortClassInfo Create(Class @class)
        {
            return new ShortClassInfo
            {
                Id = @class.Id,
                Name = @class.Name
            };
        }

    }
}
