using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoLPGalleryCategoryStorage : BaseDemoIntStorage<LPGalleryCategory>
    {
        public DemoLPGalleryCategoryStorage() : base(x => x.Id) {}
    }

    public class DemoLPGalleryCategoryService : DemoSchoolServiceBase, ILPGalleryCategoryService
    {
        private DemoLPGalleryCategoryStorage lpGalleryCategoryStorage;
        public DemoLPGalleryCategoryService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            lpGalleryCategoryStorage = new DemoLPGalleryCategoryStorage();
        }

        public LPGalleryCategory Add(string name)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            BaseSecurity.EnsureAdminOrTeacher(Context);
            ValidateParams(name);
            return lpGalleryCategoryStorage.Add(new LPGalleryCategory {Name = name, OwnerRef = Context.PersonId.Value});
        }

        public LPGalleryCategory Edit(int categoryId, string name)
        {
            ValidateParams(name);
            var category = lpGalleryCategoryStorage.GetById(categoryId);
            EnsureModifyPermission(category);
            category.Name = name;
            lpGalleryCategoryStorage.Update(category);
            return category;
        }

        public bool Exists(string name, int? excludeCategoryId)
        {
            return GetList().Any(c => c.Name == name && c.Id != excludeCategoryId);
        }

        public void Delete(int categoryId)
        {
            EnsureModifyPermission(lpGalleryCategoryStorage.GetById(categoryId));
            lpGalleryCategoryStorage.Delete(categoryId);
        }

        public IList<LPGalleryCategory> GetList()
        {
            return lpGalleryCategoryStorage.GetAll();
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
    }
}
