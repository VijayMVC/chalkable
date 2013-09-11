REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.AppGradeLevel');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.id.SchoolPersonId');



NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.AppMarketService */
    CLASS(
        'AppMarketService', EXTENDS(chlk.services.BaseService), [


            [[ArrayOf(chlk.models.apps.AppCategory), ArrayOf(chlk.models.apps.AppGradeLevel),
                String, Number, Number, Number, Number]],

            ria.async.Future, function getApps(categories, gradeLevels,
                filter, filterMode_, sortingMode_, start_, count_) {


                var gradeLvlsIds = gradeLevels.map(function(item){
                    return item.getId().valueOf()
                }).join(',');

                var categoryIds = categories.map(function(item){
                    return item.getId().valueOf()
                }).join(',');
                return this
                    .getPaginatedList('AppMarket/List.json', chlk.models.apps.AppMarketApplication, {
                        start: start_ | 0,
                        count: count_,
                        categoriesIds:  categoryIds,
                        gradeLevelsIds: gradeLvlsIds,
                        filter: filter,
                        filterMode: filterMode_,
                        sortingMode: sortingMode_
                    })
                    .then(function(data){
                        var items = data.getItems();
                        for(var i = 0; i < 19; ++i){
                            var app =  new chlk.models.apps.AppMarketApplication();
                            app.setName("App test");
                            app.setId(new chlk.models.id.AppId('dab27768-6a5d-41d5-82b1-d943ef002eae'));
                            app.setShortDescription("rskldfj;alskdfja;skldjfa;sldkfja;sdfsdfsdfsdfsdfldkfjasl;");
                            app.setSmallPictureId(new chlk.models.id.PictureId("90e359b7-7199-4296-8148-a072bcd67bb3"));
                            items.push(app);
                        }
                        data.setPageIndex(1);
                        data.setPageSize(10);
                        data.setTotalPages(2);
                        data.setItems(items);
                        return data;
                    });
            },

            [[chlk.models.id.SchoolPersonId, Number]],
            ria.async.Future, function getInstalledApps(personId, start_) {
                //return this.getPaginatedList('Application/List.json', chlk.models.apps.AppMarketApplication, {
                return this
                    .getPaginatedList('AppMarket/ListInstalled.json', chlk.models.apps.AppMarketApplication, {
                        personId: personId.valueOf(),
                        start: start_ | 0
                    })
                    .then(function(data){
                        var items = data.getItems();

                        for(var i = 0; i < 9; ++i){
                            var app =  new chlk.models.apps.AppMarketApplication();
                            app.setName("App test");
                            app.setId(new chlk.models.id.AppId('dab27768-6a5d-41d5-82b1-d943ef002eae'));
                            app.setShortDescription("rskldfj;alskdfja;skldjfa;sldkfja;sdfsdfsdfsdfsdfldkfjasl;");
                            app.setSmallPictureId(new chlk.models.id.PictureId("90e359b7-7199-4296-8148-a072bcd67bb3"));
                            items.push(app);
                        }
                        data.setPageIndex(1);
                        data.setPageSize(10);
                        data.setTotalPages(2);
                        data.setItems(items);
                        return data;
                    });
            },
            [[chlk.models.id.AppId]],
            ria.async.Future, function getDetails(appId) {
                return this
                    .get('AppMarket/Read.json', chlk.models.apps.AppMarketApplication, {
                        applicationId: appId.valueOf()
                    });
                /*
                var app  =  new chlk.models.apps.AppMarketApplication();
                app.setName("App test");
                app.setUrl('https://localhost/apptest');
                app.setId(new chlk.models.id.AppId('dab27768-6a5d-41d5-82b1-d943ef002eae'));
                app.setShortDescription("rskldfj;alskdfja;skldjfa;sldkfja;sdfsdfsdfsdfsdfldkfjasl;");
                app.setSmallPictureId(new chlk.models.id.PictureId("90e359b7-7199-4296-8148-a072bcd67bb3"));
                var devInfo = new chlk.models.developer.DeveloperInfo();
                devInfo.setName('Developerovich');
                app.setDeveloperInfo(devInfo);
                return new ria.async.DeferredData(app);
                */
            }
        ])
});