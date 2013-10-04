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
            function $(apps, categories, gradelevels, balance){

                BASE(categories, gradelevels, balance);
                var items = apps.getItems();
                var firstApp = new chlk.models.apps.AppMarketApplication();



                //todo take only 3 screenshots
                var screenshotPictures = [];
                if (items.length > 0){
                    items.forEach(function(item){
                        var itemScreenshots = item.getScreenshotPictures().getItems() || [];
                        for(var i = 0; i < itemScreenshots.length; ++i){
                            screenshotPictures.push(itemScreenshots[i]);
                        }
                    })
                }
                firstApp.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshotPictures, true));
                this.setFirstApp(firstApp);
                this.setApps(apps);
            }

        ]);


});
