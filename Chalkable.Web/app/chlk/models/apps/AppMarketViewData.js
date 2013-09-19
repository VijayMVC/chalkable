REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketViewData*/
    CLASS(
        'AppMarketViewData', [
            chlk.models.apps.AppMarketApplication, 'firstApp',
            chlk.models.common.PaginatedList, 'apps',

            [[chlk.models.common.PaginatedList]],
            function $(apps){
                BASE();

                var items = apps.getItems();
                if (items.length > 0){
                    this.setFirstApp(items[0]);
                }else{
                    var firstApp = new chlk.models.apps.AppMarketApplication();
                    firstApp.setScreenshotIds([]);
                    this.setFirstApp(firstApp);
                }
                this.setApps(apps);
            }

        ]);


});
