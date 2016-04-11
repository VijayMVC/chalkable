using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class LPGalleryCategoryDataAccess : DataAccessBase<LPGalleryCategory, int>
    {
        public LPGalleryCategoryDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public override IList<LPGalleryCategory> GetAll(QueryCondition conditions = null)
        {
            return ReadMany<LPGalleryCategory>(BuildSelectQuery(conditions));
        }

        public override LPGalleryCategory GetById(int key)
        {
            var conds = new AndQueryCondition {{LPGalleryCategory.ID_FIELD, key}};
            return ReadOne<LPGalleryCategory>(BuildSelectQuery(conds));
        }

        public override LPGalleryCategory GetByIdOrNull(int key)
        {
            var conds = new AndQueryCondition { { LPGalleryCategory.ID_FIELD, key } };
            return ReadOneOrNull<LPGalleryCategory>(BuildSelectQuery(conds));
        }
        private DbQuery BuildSelectQuery(QueryCondition condition)
        {
            return Orm.SimpleSelect(LPGalleryCategory.VW_LP_GALLERY_CATEGORY, condition);
        }

        public bool Exists(string name, int? excludedCategoryId)
        {
            var conds = new AndQueryCondition { { LPGalleryCategory.NAME_FIELD, name } };
            if (excludedCategoryId.HasValue)
                conds.Add(LPGalleryCategory.ID_FIELD, excludedCategoryId.Value, ConditionRelation.NotEqual);
            return Exists(conds);
        }
    }
}
