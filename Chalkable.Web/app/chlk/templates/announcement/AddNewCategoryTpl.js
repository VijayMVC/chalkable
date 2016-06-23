REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.CategoriesListViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AddNewCategoryTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.announcement.CategoriesListViewData)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AddNewCategory.jade')],
        'AddNewCategoryTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.CategoryViewData), 'categories'
        ])
});