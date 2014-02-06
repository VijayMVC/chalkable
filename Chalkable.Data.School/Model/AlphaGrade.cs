using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class AlphaGrade
    {
        public int Id { get; set; }
        public int SchoolRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
