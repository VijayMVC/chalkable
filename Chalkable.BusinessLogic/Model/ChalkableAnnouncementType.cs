namespace Chalkable.BusinessLogic.Model
{
    public class ChalkableAnnouncementType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
        public static ChalkableAnnouncementType[] All
        {
            get { return all; }
        }

        private static readonly ChalkableAnnouncementType[] all = new[]
            {
                    new ChalkableAnnouncementType
                    {
                        Id = 1,
                        Name = "Announcement",
                        Keywords = "announcement"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 2,
                        Name = "HW",
                        Keywords = "hw,homework"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 3,
                        Name = "Essay",
                        Keywords = "essay"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 4,
                        Name = "Quiz",
                        Keywords = "quiz"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 5,
                        Name = "Test",
                        Keywords = "test"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 6,
                        Name = "Project",
                        Keywords = "project"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 7,
                        Name = "Final",
                        Keywords = "final"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 8,
                        Name = "Midterm",
                        Keywords = "midterm"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 9,
                        Name = "Book Report",
                        Keywords = "book,report"
                    }, 
                    new ChalkableAnnouncementType
                    {
                        Id = 10,
                        Name = "Term paper",
                        Keywords = "term,paper"
                    }, 
            };

    }
}
