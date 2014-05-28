using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
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
                    Description = "Academic Achievement",
                    Gradable = true,
                    Id = DemoSchoolConstants.AlgebraAcademicAchievementId,
                    Name = "Academic Achievement",
                    Percentage = 50
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    Description = "Academic Practice",
                    Gradable = true,
                    Id = DemoSchoolConstants.AlgebraAcademicPracticeId,
                    Name = "Task",
                    Percentage = 50
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    Description = "Academic Achievement",
                    Gradable = true,
                    Id = DemoSchoolConstants.GeometryAcademicAchievementId,
                    Name = "Academic Achievement",
                    Percentage = 50
                },
                new ClassAnnouncementType
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    Description = "Academic Practice",
                    Gradable = true,
                    Id = DemoSchoolConstants.GeometryAcademicPracticeId,
                    Name = "Academic Practice",
                    Percentage = 50
                }
            };

            Add(clsTypes);
        }
    }
}
