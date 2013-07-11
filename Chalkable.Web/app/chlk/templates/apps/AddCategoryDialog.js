REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.apps.AppCategory');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AddCategoryDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/add-app-category-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppCategory)],
        'AddCategoryDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'name',
            [ria.templates.ModelBind],
            String, 'description'
        ])
});