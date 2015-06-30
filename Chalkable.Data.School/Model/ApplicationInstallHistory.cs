namespace Chalkable.Data.School.Model
{
    public class ApplicationInstallHistory
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int InstalledCount { get; set; }
    }
}