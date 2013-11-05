REQUIRE('chlk.templates.apps.AppMarketBaseTpl');
REQUIRE('chlk.models.apps.AppMarketViewData');
REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.apps.AppMarketScreenshot');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarket*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketViewData)],
        'AppMarket', EXTENDS(chlk.templates.apps.AppMarketBaseTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'apps',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppMarketScreenshot), 'marketScreenshots',

            [[ArrayOf(chlk.models.apps.AppCategory)]],
            function getAllCategoriesIds(categories){
                return categories.map(function(item){
                    return item.getId().valueOf();
                }).join(',');
            }

        ])
});