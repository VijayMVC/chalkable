REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.apps.AppMarketViewData');
REQUIRE('chlk.models.apps.AppMarketApplication');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketViewData)],
        'AppMarket', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'firstApp'

        ])
});