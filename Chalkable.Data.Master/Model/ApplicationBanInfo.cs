using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.Master.Model
{
    public class ApplicationBanInfo
    {
        public Guid ApplicationId { get; set; }
        public int UnBannedSchoolCount { get; set; }
        public int BannedSchoolCount { get; set; }
        public bool BannedForCurrentSchool { get; set; }     
    }
}
