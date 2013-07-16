REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.apps.AppCategory');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AddCategoryDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/add-app-category-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppCategory)],
        'AddCategoryDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'description'
        ])
});