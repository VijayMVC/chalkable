REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AttachAppDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/attach-app-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'AttachAppDialog', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppMarketApplication), 'items'
        ])
});