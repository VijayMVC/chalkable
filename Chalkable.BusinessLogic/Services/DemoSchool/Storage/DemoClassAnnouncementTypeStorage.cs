using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassAnnouncementTypeStorage:BaseDemoStorage<int, ClassAnnouncementType>
    {
        public DemoClassAnnouncementTypeStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<ClassAnnouncementType> GetAll(int classId)
        {
            return data.Where(x => x.Value.ClassRef == classId).Select(x => x.Value).ToList();
        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            foreach (var classAnnouncementType in classAnnouncementTypes.Where(classAnnouncementType => !data.ContainsKey(classAnnouncementType.Id)))
            {
                data[classAnnouncementType.Id] = classAnnouncementType;
            }
        }

        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            foreach (var classAnnouncementType in classAnnouncementTypes)
            {
                if (data.ContainsKey(classAnnouncementType.Id))
                    data[classAnnouncementType.Id] = classAnnouncementType;
            }
        }


        public override void Setup()
        {
            var clsTypes = new List<ClassAnnouncementType>
            {
                new ClassAnnouncementType
                {
                    ClassRef = 1,
                    Description = "Type 1",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Type 1",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = 1,
                    Description = "Type 2",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Type 2",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = 2,
                    Description = "Type 1",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Type 1",
                    Percentage = 100
                },
                new ClassAnnouncementType
                {
                    ClassRef = 2,
                    Description = "Type 2",
                    Gradable = true,
                    Id = GetNextFreeId(),
                    Name = "Type 2",
                    Percentage = 100
                }
            };

            Add(clsTypes);
        }
    }
}
