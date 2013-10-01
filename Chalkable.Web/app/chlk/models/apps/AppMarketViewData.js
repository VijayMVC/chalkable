REQUIRE('chlk.models.apps.AppMarketBaseViewData');
REQUIRE('chlk.models.apps.AppSortingMode');

NAMESPACE('chlk.models.apps', function () {
    "use strict";


    /** @class chlk.models.apps.AppMarketViewData*/
    CLASS(
        'AppMarketViewData', EXTENDS(chlk.models.apps.AppMarketBaseViewData), [
            chlk.models.apps.AppMarketApplication, 'firstApp',
            chlk.models.common.PaginatedList, 'apps',

            [[
                chlk.models.common.PaginatedList,
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                Number
            ]],
            function $(apps, categories, gradelelevels, balance){
                BASE(categories, gradelelevels, balance);

                var items = apps.getItems();
                if (items.length > 0){
                    this.setFirstApp(items[0]);
                }else{
                    //todo: show all screenshots
                    //set default placeholder screenshot id
                    var firstApp = new chlk.models.apps.AppMarketApplication();
                    firstApp.setScreenshotIds([]);
                    this.setFirstApp(firstApp);
                }
                this.setApps(apps);
            }

        ]);


});
