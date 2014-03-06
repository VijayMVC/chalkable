using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels(int? schoolId = null);
        void AddGradeLevel(int id, string name, int number);
        void AddGradeLevels(IList<GradeLevel> gradeLevels);
        void DeleteGradeLevels(IList<int> ids);
        GradeLevel AddSchoolGradeLevel(int gradeLevelId, int schoolId);
        GradeLevel DeleteSchoolGradeLevel(int gradeLevelId, int schoolId);
        IList<GradeLevel> CreateDefault();
    }
    public class GradeLevelService : SchoolServiceBase, IGradeLevelService
    {
        public GradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradeLevel> GetGradeLevels(int ? schoolId = null)
        {
            using (var uow = Read())
            {
                var da = new GradeLevelDataAccess(uow);
                return da.GetGradeLevels(schoolId);
            }
        }

        public void AddGradeLevel(int id, string name, int number)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Insert(new GradeLevel{Id = id, Name = name, Number = number});
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
                            Id = i, 
                            Name = i.ToString(CultureInfo.InvariantCulture),
                            Number = i
                        });
                }
                new GradeLevelDataAccess(uow).Insert(gradeLevels);
                uow.Commit();
                return gradeLevels;
            }
        }


        public GradeLevel AddSchoolGradeLevel(int gradeLevelId, int schoolId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolGradeLevelDataAccess(uow, null)
                    .Insert(new SchoolGradeLevel
                        {
                            GradeLevelRef = gradeLevelId,
                            SchoolRef = schoolId,
                        });
                var gl = new GradeLevelDataAccess(uow).GetById(gradeLevelId);
                uow.Commit();
                return gl;
            }
        }

        public GradeLevel DeleteSchoolGradeLevel(int gradeLevelId, int schoolId)
        {
            if(!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolGradeLevelDataAccess(uow, schoolId).DeleteSchoolGradeLevel(gradeLevelId);
                var gl = new GradeLevelDataAccess(uow).GetById(gradeLevelId);
                uow.Commit();
                return gl;
            }
            
        }

        public void AddGradeLevels(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Insert(gradeLevels);
                uow.Commit();
            }
        }

        public void DeleteGradeLevels(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }
    }
}
