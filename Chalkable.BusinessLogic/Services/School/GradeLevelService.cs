using System;
using System.Collections.Generic;
using System.Globalization;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels();
        void AddGradeLevel(string name, int number);
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

        public void AddGradeLevel(string name, int number)
        {
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Insert(new GradeLevel{Id = Guid.NewGuid(), Name = name, Number = number});
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
                    gradeLevels.Add(new GradeLevel
                        {
                            Id = Guid.NewGuid(), 
                            Name = i.ToString(CultureInfo.InvariantCulture),
                            Number = i
                        });
                }
                new GradeLevelDataAccess(uow).Insert(gradeLevels);
                uow.Commit();
                return gradeLevels;
            }
        }
    }
}
