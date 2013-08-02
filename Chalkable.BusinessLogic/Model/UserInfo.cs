using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model
{
    public class UserInfo
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
