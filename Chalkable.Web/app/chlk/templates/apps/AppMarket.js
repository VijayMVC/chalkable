REQUIRE('chlk.templates.apps.AppMarketBaseTpl');
REQUIRE('chlk.models.apps.AppMarketViewData');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketViewData)],
        'AppMarket', EXTENDS(chlk.templates.apps.AppMarketBaseTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'firstApp',


            [[ArrayOf(chlk.models.apps.AppCategory)]],
            function getAllCategoriesIds(categories){
                return categories.map(function(item){
                    return item.getId().valueOf();
                }).join(',');
            }

        ])
});