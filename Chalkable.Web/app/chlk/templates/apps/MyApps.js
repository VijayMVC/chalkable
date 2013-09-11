REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.apps.AppMarketApplication');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/my-apps.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'MyApps', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppMarketApplication), 'items'
        ])
});