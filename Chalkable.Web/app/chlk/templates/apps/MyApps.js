REQUIRE('chlk.models.apps.MyAppsViewData');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.MyApps*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/my-apps.jade')],
        [ria.templates.ModelBind(chlk.models.apps.MyAppsViewData)],
        'MyApps', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'editable'
        ])
});