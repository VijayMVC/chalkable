REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.id.AppCategoryId');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.CategoryDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-category-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppCategory)],
        'CategoryDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppCategoryId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'description'
        ])
});