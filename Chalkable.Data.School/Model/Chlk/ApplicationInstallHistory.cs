using System;

namespace Chalkable.Data.School.Model.Chlk
{
    public class ApplicationInstallHistory
    {
        public int ApplicationInstallActionId { get; set; }
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OwnerRoleId { get; set; }
        public DateTime Date { get; set; }
        public bool Installed { get; set; }
        public int PersonCount { get; set; }
    }
}