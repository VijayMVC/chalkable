using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels();
        void AddGradeLevel(string name);
        IList<GradeLevel> CreateDefault();
    }
    public class GradeLevelService : SchoolServiceBase, IGradeLevelService
    {
        public GradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            using (var uow = Read())
            {
                var da = new GradeLevelDataAccess(uow);
                return da.GetAll();
            }
        }

        public void AddGradeLevel(string name)
        {
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Insert(new GradeLevel{Id = Guid.NewGuid(), Name = name});
                uow.Commit();
            }
        }

        public IList<GradeLevel> CreateDefault()
        {
            using (var uow = Update())
            {
                var gradeLevels = new List<GradeLevel>();
                var max = 12;
                for (int i = 1; i < max; i++)
                {
                    gradeLevels.Add(new GradeLevel { Id = Guid.NewGuid(), Name = i.ToString() });
                }
                new GradeLevelDataAccess(uow).Insert(gradeLevels);
                uow.Commit();
                return gradeLevels;
            }
        }
    }
}
