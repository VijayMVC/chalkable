using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoAlphaGradeService : DemoSchoolServiceBase, IAlphaGradeService
    {
        public DemoAlphaGradeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.AlphaGradeStorage.Add(alphaGrades);
            
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.AlphaGradeStorage.Update(alphaGrades);

        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.AlphaGradeStorage.Delete(id);
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.AlphaGradeStorage.Delete(ids);
        }

        public IList<AlphaGrade> GetAlphaGrades()
        {
            return Storage.AlphaGradeStorage.GetAll();
        }


        public IList<AlphaGrade> GetAlphaGradesForClass(int classId)
        {
            return Storage.AlphaGradeStorage.GetForClass(classId);
        }

        public IList<AlphaGrade> GetAlphaGradesForClassStandards(int classId)
        {
            return Storage.AlphaGradeStorage.GetForClassStandarts(classId);
        }
    }
}
