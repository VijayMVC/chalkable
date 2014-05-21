using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassAnnouncementTypeStorage:BaseDemoIntStorage<ClassAnnouncementType>
    {
        public DemoClassAnnouncementTypeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<ClassAnnouncementType> GetAll(int classId)
        {
            return data.Where(x => x.Value.ClassRef == classId).Select(x => x.Value).ToList();
        }

     
        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            Update(classAnnouncementTypes);
        }


        public override void Setup()
        {
            var clsTypes = new List<ClassAnnouncementType>
            {
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    Description = "Announcement",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Announcement",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    Description = "Task",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Task",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    Description = "Announcement",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Announcement",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    Description = "Task",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Task",
                    Percentage = 100
                }
            };

            Add(clsTypes);
        }
    }
}
