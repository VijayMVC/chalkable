using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStandardService
    {
        Standard AddStandard(int id, string name, string description, int standardSubjectId, int? parentStandardId, int? lowerGradeLevelId, int? upperGradeLevelId, bool isActive);
        void AddStandards(IList<Standard> standards);
        void DeleteStandard(int id);
        void DeleteStandards(IList<int> ids);
        Standard GetStandardById(int id);
        IList<Standard> GetStandardes(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true);

        IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards); 
        void AddStandardSubjects(IList<StandardSubject> standardSubjects);
        IList<StandardSubject> GetStandardSubjects();
        //IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId);

    }
    public class StandardService : SchoolServiceBase, IStandardService
    {
        public StandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Standard AddStandard(int id, string name, string description, int standardSubjectId, int? parentStandardId
            , int? lowerGradeLevelId, int? upperGradeLevelId, bool isActive)
        {
            var standard = new Standard
                {
                    Id = id,
                    Description = description,
                    Name = name,
                    IsActive = isActive,
                    LowerGradeLevelRef = lowerGradeLevelId,
                    UpperGradeLevelRef = upperGradeLevelId,
                    ParentStandardRef = parentStandardId,
                    StandardSubjectRef = standardSubjectId
                };
            AddStandards(new List<Standard> {standard});
            return standard;
        }

        public void AddStandards(IList<Standard> standards)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            if (standards.Any(IsInvalidStandardModel))
                throw new ChalkableException("Invalid params. LowerGradeLevelId can not be greater than upperGradeLevelId");
            
            using (var uow = Update())
            {
                new StandardDataAccess(uow).Insert(standards);
                uow.Commit();
            }
        }

        private bool IsInvalidStandardModel(Standard standard)
        {
            return standard.LowerGradeLevelRef.HasValue && standard.UpperGradeLevelRef.HasValue
                   && standard.LowerGradeLevelRef > standard.UpperGradeLevelRef;
        }

        public void DeleteStandard(int id)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StandardDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public Standard GetStandardById(int id)
        {
            using (var uow = Read())
            {
                return new StandardDataAccess(uow).GetById(id);
            }
        }

        public IList<Standard> GetStandardes(int? classId, int? gradeLevelId, int? subjectId, int? parentStandardId = null, bool allStandards = true)
        {
            using (var uow = Read())
            {
                return new StandardDataAccess(uow).GetStandards( new StandardQuery
                    {
                        ClassId = classId,
                        GradeLavelId = gradeLevelId,
                        StandardSubjectId = subjectId,
                        ParentStandardId = parentStandardId
                    });
            }
        }

        public void AddStandardSubjects(IList<StandardSubject> standardSubjects)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            if(standardSubjects != null && standardSubjects.Count > 0)
                using (var uow = Update())
                {
                    new StandardSubjectDataAccess(uow).Insert(standardSubjects);
                    uow.Commit();
                }
        }

        public IList<StandardSubject> GetStandardSubjects()
        {
            using (var uow = Read())
            {
                return new StandardSubjectDataAccess(uow).GetAll();
            }
        }

        public IList<ClassStandard> AddClassStandards(IList<ClassStandard> classStandards)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                new ClassStandardDataAccess(uow).Insert(classStandards);
                uow.Commit();
                return classStandards;
            }
        }
        
        //public IList<AnnouncementStandardDetails> GetAnnouncementStandards(int announcementId)
        //{
        //    using (var uow = Read())
        //    {
        //       new AnnouncementStandardDataAccess(uow).
        //    }
        //}


        public void DeleteStandards(IList<int> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
