REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.CategoryViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.LpGalleryCategoryService */
    CLASS(
        'LpGalleryCategoryService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function list() {
                return this.get('LPGalleryCategory/ListCategories.json', ArrayOf(chlk.models.announcement.CategoryViewData));
            },

            [[String]],
            ria.async.Future, function create(name) {
                return this.get('LPGalleryCategory/CreateCategory.json', Boolean, {
                    name: name
                });
            },

            [[chlk.models.id.LpGalleryCategoryId, String]],
            ria.async.Future, function update(categoryId, name) {
                return this.get('LPGalleryCategory/UpdateCategory.json', Boolean, {
                    name: name,
                    categoryId: categoryId.valueOf()
                });
            },

            [[chlk.models.id.LpGalleryCategoryId]],
            ria.async.Future, function deleteCategory(categoryId) {
                return this.get('LPGalleryCategory/DeleteCategory.json', Boolean, {
                    categoryId: categoryId.valueOf()
                });
            },

            [[ArrayOf(chlk.models.announcement.CategoryViewData)]],
            function cacheLessonPlanCategories(categories){
                this.getContext().getSession().set(ChlkSessionConstants.LESSON_PLAN_CATEGORIES, categories);
            },

            ArrayOf(chlk.models.announcement.CategoryViewData), function getLessonPlanCategoriesSync(){
                return this.getContext().getSession().get(ChlkSessionConstants.LESSON_PLAN_CATEGORIES, []);
            },

            function emptyLessonPlanCategoriesCache(){
                this.cacheLessonPlanCategories(null);
            }
    ]);
});