REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.apps.AppCategory');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.CategoryDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-category-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppCategory)],
        'CategoryDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppCategoryId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'description'
        ])
});