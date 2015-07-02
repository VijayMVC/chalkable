using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILPGalleryCategoryService
    {
        LPGalleryCategory Add(string name);
        LPGalleryCategory Edit(int categoryId, string name);
        void Delete(int categoryId);
        IList<LPGalleryCategory> GetList();
    }

    public class LPGalleryCategoryService : SchoolServiceBase, ILPGalleryCategoryService
    {
        public LPGalleryCategoryService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public LPGalleryCategory Add(string name)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            ValidateParams(name); 
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var res = new LPGalleryCategory {Name = name, OwnerRef = Context.PersonId.Value};
            using (var u = Update())
            {
                var da = new DataAccessBase<LPGalleryCategory, int>(u);
                da.Insert(res);
                res = da.GetAll().Last();
                u.Commit();
                return res;
            }
        }

        public LPGalleryCategory Edit(int categoryId, string name)
        {
            ValidateParams(name);
            using (var uow = Update())
            {
                var da = new DataAccessBase<LPGalleryCategory, int>(uow);
                var category = da.GetById(categoryId);
                EnsureModifyPermission(category);
                category.Name = name;
                da.Update(category);
                uow.Commit();
                return category;
            }
        }

        public void Delete(int categoryId)
        {
            DoUpdate(u =>
                {
                    var da = new DataAccessBase<LPGalleryCategory, int>(u);
                    EnsureModifyPermission(da.GetById(categoryId));
                    da.Delete(categoryId);
                    //TODO: delete all references from lesson plan to category
                });
        }
        
        private void ValidateParams(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ChalkableException(string.Format(ChlkResources.ERR_PARAM_IS_MISSING_TMP, "Name ")); 
        }

        private void EnsureModifyPermission(LPGalleryCategory category)
        {
            if (category.OwnerRef != Context.PersonId)
                throw new ChalkableSecurityException("Only owner can modify category");            
        }

        public IList<LPGalleryCategory> GetList()
        {
            return DoRead(u => new DataAccessBase<LPGalleryCategory>(u).GetAll());
        }
    }
}
