REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppSortingMode');

NAMESPACE('chlk.models.apps', function () {
    "use strict";


    /** @class chlk.models.apps.AppMarketViewData*/
    CLASS(
        'AppMarketViewData', [
            chlk.models.apps.AppMarketApplication, 'firstApp',
            chlk.models.common.PaginatedList, 'apps',
            Number, 'currentBalance',

            [[chlk.models.common.PaginatedList, Number]],
            function $(apps, balance){
                BASE();



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
                this.setCurrentBalance(balance);
                this.setApps(apps);
            }

        ]);


});
