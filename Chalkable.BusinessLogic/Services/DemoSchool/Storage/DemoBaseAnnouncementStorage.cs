namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoBaseAnnouncementStorage
    {
        public DemoBaseAnnouncementStorage(DemoStorage storage)
        {
        }
    }


    public class DemoTeacherAnnouncementStorage:DemoBaseAnnouncementStorage
    {
        public DemoTeacherAnnouncementStorage(DemoStorage storage) : base(storage)
        {
        }
    }

    public class DemoAnnouncementStudentStorage : DemoBaseAnnouncementStorage
    {
        public DemoAnnouncementStudentStorage(DemoStorage storage)
            : base(storage)
        {
        }
    }

    public class DemoAdminAnnouncementStorage : DemoBaseAnnouncementStorage
    {
        public DemoAdminAnnouncementStorage(DemoStorage storage)
            : base(storage)
        {
        }
    }
}
