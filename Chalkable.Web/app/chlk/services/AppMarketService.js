REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.AppGradeLevel');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.apps.AppInstallPostData');
REQUIRE('chlk.models.people.Role');


REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');



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
                    .getPaginatedList('AppMarket/List.json', chlk.models.apps.AppMarketApplication, {
                        start: start_ | 0,
                        count: count_,
                        categoriesIds:  categoryIds,
                        gradeLevelsIds: gradeLvlsIds,
                        filter: filter,
                        filterMode: filterMode_ && filterMode_.valueOf(),
                        sortingMode: sortingMode_ && sortingMode_.valueOf()
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
                    })
                    .then(function(app){
                        //todo:remove this later
                        var roles = [];

                        var studentRole = new chlk.models.people.Role();
                        studentRole.setName('Student');
                        studentRole.setId(3);

                        var teacherRole = new chlk.models.people.Role();
                        teacherRole.setName('Teacher');
                        teacherRole.setId(2);

                        var adminRole = new chlk.models.people.Role();
                        adminRole.setName('Admin');
                        adminRole.setId(7);

                        roles.push(studentRole);
                        roles.push(adminRole);
                        roles.push(teacherRole);

                        app.setValidRoles(roles);
                        return app;
                    });
            },

            [[
                chlk.models.id.AppId,
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                chlk.models.id.AppInstallGroupId
            ]],
            ria.async.Future, function installApp(appId, departments, classes, roles, gradeLevels, currentPerson_) {
                return this
                    .post('AppMarket/Install.json', Boolean, {
                        applicationId: appId.valueOf(),
                        personId: currentPerson_ && currentPerson_.valueOf(),
                        departmentids: this.arrayToCsv(departments),
                        classids: this.arrayToCsv(classes),
                        roleIds: this.arrayToCsv(roles),
                        gradelevelids: this.arrayToCsv(gradeLevels)
                    });
            },

            [[String]],
            ria.async.Future, function uninstallApps(ids) {
                return this
                    .post('AppMarket/Uninstall.json', Boolean, {
                        applicationInstallIds: ids
                    });
            }
        ])
});