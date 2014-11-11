REQUIRE('chlk.models.apps.AppMarketBaseViewData');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppMarketScreenshot');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppMarketViewData*/
    CLASS(
        'AppMarketViewData', EXTENDS(chlk.models.apps.AppMarketBaseViewData), [

            ArrayOf(chlk.models.apps.AppMarketScreenshot), 'marketScreenshots',
            chlk.models.common.PaginatedList, 'apps',
            [[
                chlk.models.common.PaginatedList,
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                Number
            ]],
            function $(apps, categories, gradelevels, balance){

                BASE(categories, gradelevels, balance);
                var items = apps.getItems();
                var marketScreenshots = [];

                for (var i = 0; i < items.length; i++) {
                    var app = items[i];
                    var appScreenshots = app.getScreenshotPictures().getItems() || [];

                    if (appScreenshots.length > 0){
                        var screenshot = appScreenshots[0];
                        screenshot.setTitle(app.getName());

                        var marketScreenshot = new chlk.models.apps.AppMarketScreenshot(screenshot, app.getId());
                        marketScreenshots.push(marketScreenshot);
                    }
                    if (i == 2) break;
                }
                this.setMarketScreenshots(marketScreenshots);
                this.setApps(apps);
            }
        ]);


});
