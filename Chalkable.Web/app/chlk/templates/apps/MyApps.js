REQUIRE('chlk.models.apps.MyAppsViewData');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.MyApps*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/my-apps.jade')],
        [ria.templates.ModelBind(chlk.models.apps.MyAppsViewData)],
        'MyApps', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            Boolean, 'editable'
        ])
});