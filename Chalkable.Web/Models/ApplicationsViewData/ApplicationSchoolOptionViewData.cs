using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationSchoolOptionViewData
    {
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        public bool Banned { get; set; }

        public static ApplicationSchoolOptionViewData Create(ApplicationSchoolBan appSchoolBan)
        {
            return new ApplicationSchoolOptionViewData
            {
                SchoolId = appSchoolBan.SchoolRef,
                Banned = appSchoolBan.Banned,
                SchoolName = appSchoolBan.SchoolName
            };
        }
    }
}