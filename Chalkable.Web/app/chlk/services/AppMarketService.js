REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.AppCategoryService');

REQUIRE('ria.async.Future');

REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');

REQUIRE('chlk.models.apps.AppGradeLevel');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppCategory');


REQUIRE('chlk.models.people.Role');
REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.apps.InstalledApp');


NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.AppMarketService */
    CLASS(
        'AppMarketService', EXTENDS(chlk.services.BaseService), [

            [[String]],
            ria.async.Future, function getAppsByFilter(filter) {
                var categoriesService = this.getContext().getService(chlk.services.AppCategoryService);
                var gradeLevels = this.getContext().getService(chlk.services.GradeLevelService).getGradeLevels();
                return categoriesService
                    .getCategories()
                    .then(function(categories){
                        return this.getApps(categories.getItems(), gradeLevels, filter)
                            .then(function(result){
                                return result.getItems();
                            });
                    }, this);
            },

            [[
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                String,
                chlk.models.apps.AppPriceType,
                chlk.models.apps.AppSortingMode,
                Number,
                Number
            ]],
            ria.async.Future, function getApps(categories, gradeLevels,
                filter, filterMode_, sortingMode_, start_, count_) {


                var gradeLvlsIds = gradeLevels.map(function(item){
                    return item.getId().valueOf()
                }).join(',');

                var categoryIds = categories.map(function(item){
                    return item.getId().valueOf()
                }).join(',');


                return this
                    .getPaginatedList('AppMarket/List.json', chlk.models.apps.Application, {
                        start: start_ | 0,
                        count: count_ || 20,
                        categoriesIds:  categoryIds,
                        gradeLevelsIds: gradeLvlsIds,
                        filter: filter,
                        filterMode: filterMode_ && filterMode_.valueOf(),
                        sortingMode: sortingMode_ && sortingMode_.valueOf()
                    });/*.then(function(data){
                        var items = data.getItems();

                        for(var i = 0; i < 10; ++i){
                            var app =  new chlk.models.apps.AppMarketApplication();
                            app.setName("App test");
                            app.setId(new chlk.models.id.AppId('dab27768-6a5d-41d5-82b1-d943ef002eae'));
                            app.setShortDescription("rskldfj;alskdfja;skldjfa;sldkfja;sdfsdfsdfsdfsdfldkfjasl;");
                            app.setSmallPictureId(new chlk.models.id.PictureId("90e359b7-7199-4296-8148-a072bcd67bb3"));
                            items.push(app);
                        }
                        data.setActualCount(10);
                        data.setItems(items);
                        data.setTotalCount(50);
                        data.setPageSize(10);
                        data.setPageIndex(start_ / 10);
                        return data;
                    }, this)*/;
            },


            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId, chlk.models.id.MarkingPeriodId, Number, Number]],
            ria.async.Future, function getAppsForAttach(personId, classId_, markingPeriodId_,  start_, count_){
                return this.getPaginatedList('AppMarket/ListInstalledForAttach.json', chlk.models.apps.Application,{
                    personId: personId.valueOf(),
                    start: start_ || 0,
                    classId: classId_ && classId_.valueOf(),
                    markingPeriodId: markingPeriodId_ && markingPeriodId_.valueOf(),
                    count: count_ || 7
                });
            },

            [[chlk.models.id.SchoolPersonId, Number, Number]],
            ria.async.Future, function getAppsForAttachToAdminAnn(personId, start_, count_){
                return this
                    .getPaginatedList('AppMarket/ListInstalledForAdminAttach.json', chlk.models.apps.Application, {
                        personId: personId.valueOf(),
                        start: start_ | 0,
                        count: count_,
                    });
            },

            [[chlk.models.id.ClassId, chlk.models.id.MarkingPeriodId, String, String, Number, Boolean]],
            ria.async.Future, function getSuggestedApps(classId, markingPeriodId, academicBenchmarkIds, start_, count_, myAppsOnly_){
                var mp = this.getContext().getSession().get('markingPeriod');
                return this.get('AppMarket/SuggestedApps.json', ArrayOf(chlk.models.apps.Application),{
                    classId : classId.valueOf(),
                    markingPeriodId: markingPeriodId ? markingPeriodId.valueOf() : (mp.getId() ? mp.getId().valueOf() : ''),
                    abIds : academicBenchmarkIds,
                    start: start_ | 0,
                    count: count_ || 9999,
                    myAppsOnly: myAppsOnly_
                });
            },

            [[Number, Number]],
            ria.async.Future, function getMyApps(start_, count_) {
                return this.getPaginatedList('AppMarket/List.json', chlk.models.apps.Application, {
                        start: start_ | 0,
                        count: count_ || 10000,
                    });
            },
        ])
});