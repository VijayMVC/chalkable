namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public interface IDemoAnnouncementStorage
    {

    }


    public class DemoBaseAnnouncementStorage
    {
        public DemoBaseAnnouncementStorage(DemoStorage storage)
        {
        }
    }


    


    public class DemoTeacherAnnouncementStorage:DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
    {
        public DemoTeacherAnnouncementStorage(DemoStorage storage) : base(storage)
        {
        }
    }

    public class DemoAnnouncementStudentStorage : DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
    {
        public DemoAnnouncementStudentStorage(DemoStorage storage)
            : base(storage)
        {
        }
    }

    public class DemoAdminAnnouncementStorage : DemoBaseAnnouncementStorage, IDemoAnnouncementStorage
    {
        public DemoAdminAnnouncementStorage(DemoStorage storage)
            : base(storage)
        {
        }
    }
}
