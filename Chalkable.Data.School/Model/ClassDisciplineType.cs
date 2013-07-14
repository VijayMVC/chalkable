using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
     public class ClassDisciplineType
    {
         public Guid Id { get; set; }
         public Guid ClassDisciplineRef { get; set; }
         public Guid DisciplineTypeRef { get; set; }
    }
}
