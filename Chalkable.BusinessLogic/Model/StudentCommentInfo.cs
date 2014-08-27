using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentCommentInfo
    {
        public Person Student { get; set; }
        public string Comment { get; set; }
    }
}
