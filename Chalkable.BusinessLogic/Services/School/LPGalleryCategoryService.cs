using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILPGalleryCategoryService
    {
        IList<LPGalleryCategory> GetList();
        LPGalleryCategory Add(string name);
        LPGalleryCategory Edit(int categoryId, string name);
        bool Exists(string name, int? excludeCategoryId);
        void Delete(int categoryId);
    }

    public class LPGalleryCategoryService : SchoolServiceBase, ILPGalleryCategoryService
    {
        public LPGalleryCategoryService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public IList<LPGalleryCategory> GetList()
        {
            return DoRead(u => new LPGalleryCategoryDataAccess(u).GetAll()).OrderBy(x=>x.Name).ToList();
        }

        public LPGalleryCategory Add(string name)
        {
            Trace.Assert(Context.PersonId.HasValue);
            ValidateParams(name); 
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var res = new LPGalleryCategory {Name = name, OwnerRef = Context.PersonId.Value};
            using (var u = Update())
            {
                var da = new LPGalleryCategoryDataAccess(u);
                if (da.Exists(name, null))
                    throw new ChalkableException("Category with such name already exists");
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
                var da = new LPGalleryCategoryDataAccess(uow);
                if(da.Exists(name, categoryId))
                    throw new ChalkableException("Category with such name already exists");
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
            Trace.Assert(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue);
            DoUpdate(u =>
                {
                    var da = new LPGalleryCategoryDataAccess(u);
                    var category = da.GetById(categoryId);
                    EnsureInDeletePermission(category);
                    da.Delete(categoryId);
                });
        }

        public bool Exists(string name, int? excludeCategoryId)
        {
            return DoRead(u => new LPGalleryCategoryDataAccess(u).Exists(name, excludeCategoryId));
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

        private void EnsureInDeletePermission(LPGalleryCategory category)
        {
            EnsureModifyPermission(category);
            if (category.LessonPlansCount > 0)
                throw new ChalkableException("Current GalleryCategory has lessonPlanTemplates. You can't delete such category");
        }
    }
}
