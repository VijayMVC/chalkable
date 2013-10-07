REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.common.PaginatedList');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketAppsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarketApps.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketViewData)],
        'AppMarketAppsTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps'
        ])
});