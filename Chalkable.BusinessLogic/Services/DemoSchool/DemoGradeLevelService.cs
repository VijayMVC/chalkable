using System.Collections.Generic;
using System.Globalization;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoGradeLevelService : DemoSchoolServiceBase, IGradeLevelService
    {
        public DemoGradeLevelService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<GradeLevel> GetGradeLevels(int ? schoolId = null)
        {
            return Storage.GradeLevelStorage.GetGradeLevels(schoolId);
        }

        public void AddGradeLevel(int id, string name, int number)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            Storage.GradeLevelStorage.Add(new GradeLevel {Id = id, Name = name, Number = number});
            
        }

        public void AddGradeLevels(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.GradeLevelStorage.Add(gradeLevels);
        }

        public void DeleteGradeLevels(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.GradeLevelStorage.Delete(ids);
        }

        public void Edit(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.GradeLevelStorage.Update(gradeLevels);
        }

        public IList<GradeLevel> CreateDefault()
        {
            var gradeLevels = new List<GradeLevel>();
            var max = 12;
            for (int i = 1; i < max; i++)
            {
                gradeLevels.Add(new GradeLevel
                {
                    Id = i,
                    Name = i.ToString(CultureInfo.InvariantCulture),
                    Number = i
                });
            }
            Storage.GradeLevelStorage.Add(gradeLevels);
            return gradeLevels;
        }


        public GradeLevel AddSchoolGradeLevel(int gradeLevelId, int schoolId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            Storage.SchoolGradeLevelStorage.Add(new SchoolGradeLevel
            {
                GradeLevelRef = gradeLevelId,
                SchoolRef = schoolId,
            });
            return Storage.GradeLevelStorage.GetById(gradeLevelId);
        }

        public GradeLevel DeleteSchoolGradeLevel(int gradeLevelId, int schoolId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.SchoolGradeLevelStorage.DeleteSchoolGradeLevel(gradeLevelId);
            return Storage.SchoolGradeLevelStorage.GetById(gradeLevelId);
        }
    }
}
