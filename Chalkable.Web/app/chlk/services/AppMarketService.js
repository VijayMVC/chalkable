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
REQUIRE('chlk.models.apps.AppTotalPrice');
REQUIRE('chlk.models.apps.ApplicationForAttach');

REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');
REQUIRE('chlk.models.apps.InstalledApp');

REQUIRE('chlk.models.funds.PersonBalance');



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
                        count: count_ || 9,
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


            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId, chlk.models.id.MarkingPeriodId,   Number, Number]],
            ria.async.Future, function getAppsForAttach(personId, classId, markingPeriodId,  start_, count_){
                return this.getPaginatedList('AppMarket/ListInstalledForAttach', chlk.models.apps.ApplicationForAttach,{
                    personId: personId.valueOf(),
                    start: start_ || 0,
                    classId: classId.valueOf(),
                    markingPeriodId: markingPeriodId.valueOf(),
                    count: count_ || 10
                });
            },

            [[chlk.models.id.SchoolPersonId, Number, String, Number]],
            ria.async.Future, function getInstalledApps(personId, start_, filter_, count_, forAttachOnly_) {
                return this
                    .getPaginatedList('AppMarket/ListInstalled.json', chlk.models.apps.AppMarketApplication, {
                        personId: personId.valueOf(),
                        start: start_ | 0,
                        filter: filter_ || "",
                        count: count_,
                        forAttach: forAttachOnly_
                    })
                    .then(function(data){
                        var apps = data.getItems() || [];
                        apps = apps.map(function(app){
                            app.setInstalledOnlyForCurrentUser(false);
                            var appInstalls = app.getApplicationInstalls() || [];

                            if (appInstalls.length == 1){
                                if(appInstalls[0].getPersonId() == appInstalls[0].getInstallationOwnerId()){
                                    app.setInstalledOnlyForCurrentUser(true);
                                }
                            }
                            return app;
                        });
                        data.setItems(apps);
                        return data;
                    })
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.MarkingPeriodId, String, String, Number]],
            ria.async.Future, function getSuggestedApps(classId, markingPeriodId, standardsCodes, start_, count_){
                return this.get('AppMarket/SuggestedApps.json', ArrayOf(chlk.models.apps.Application),{
                    classId : classId.valueOf(),
                    markingPeriodId: markingPeriodId && markingPeriodId.valueOf(),
                    standardsCodes : standardsCodes,
                    start: start_ | 0,
                    count: count_
                });
            },

            [[chlk.models.id.SchoolPersonId, Boolean, Number, String, Number]],
            ria.async.Future, function getMyApps(personId, forEdit, start_, filter_, count_) {
                return this
                    .getInstalledApps(personId, start_, filter_, count_)
                    .then(function(data){

                        var apps = data && data.getItems() || [];

                        apps = apps.map(function(app){
                            var appInstalls = app.getApplicationInstalls() || [];
                            app.setSelfInstalled(false);
                            var uninstallAppIds = [];

                            appInstalls.forEach(function(appInstall){
                                if (appInstall.isOwner() && forEdit){
                                    uninstallAppIds.push(appInstall.getAppInstallId());
                                    if(appInstall.getPersonId() == appInstall.getInstallationOwnerId()){
                                        app.setSelfInstalled(true);
                                    }
                                }
                                if (appInstall.getPersonId() == personId){
                                    app.setPersonal(true);
                                }

                            });
                            app.setUninstallable(forEdit && uninstallAppIds.length > 0);
                            var ids = uninstallAppIds.map(function(item){
                                return item.valueOf()
                            }).join(',');
                            app.setApplicationInstallIds(ids);
                            return app;
                        });

                        data.setItems(apps);

                        this.getContext().getSession().set(ChlkSessionConstants.MY_APPS_CACHED, apps);
                        return data;
                    }, this);
            },

            [[String]],
            ria.async.Future, function getMyAppsByFilter(filter) {
                var personId = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON).getId();
                return this.getMyApps(personId, false, 0, filter);
            },




            [[String]],
            chlk.models.apps.AppMarketApplication, function getMyAppByUrl(url){
                var myApps = this.getContext().getSession().get(ChlkSessionConstants.MY_APPS_CACHED) || [];
                var result = myApps.filter(function(item){
                   return item.getUrl() == url;
                }) || [];
                return result.length > 0 ? result[0] : new chlk.models.apps.AppMarketApplication();
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

                        var platforms = [];
                        var web = new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.WEB, 'Web');
                        var ios = new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.IOS, 'IOS');
                        var android = new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.ANDROID, 'Android');

                        platforms.push(web);
                        platforms.push(ios);
                        platforms.push(android);
                        app.setPlatforms(platforms);

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
                    })
                    .then(function(data){
                        this.getPersonBalance(this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON).getId(), true);
                        return data;
                    }, this)
            },

            [[
                chlk.models.id.AppId,
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                ArrayOf(chlk.models.id.AppInstallGroupId),
                chlk.models.id.AppInstallGroupId
            ]],
            ria.async.Future, function getApplicationTotalPrice(appId, departments, classes, roles, gradeLevels, currentPerson_) {
                return this
                    .post('AppMarket/GetApplicationTotalPrice.json', chlk.models.apps.AppTotalPrice, {
                        applicationId: appId.valueOf(),
                        personId: currentPerson_ && currentPerson_.valueOf(),
                        departmentids: this.arrayToCsv(departments),
                        classids: this.arrayToCsv(classes),
                        roleIds: this.arrayToCsv(roles),
                        gradelevelids: this.arrayToCsv(gradeLevels)
                    });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getPersonBalance(personId, refresh_) {
                var currentBalance = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON_BALANCE);
                return currentBalance == null || refresh_ ?
                    this.get('Fund/GetPersonBudgetBalance.json', chlk.models.funds.PersonBalance, {
                        personId: personId.valueOf()
                    })
                    .then(function(balance){
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_PERSON_BALANCE, balance);
                        return balance;
                    }, this) : ria.async.DeferredData(currentBalance);
            },

            [[String]],
            ria.async.Future, function uninstallApps(ids) {
                return this
                    .post('AppMarket/Uninstall.json', Boolean, {
                        applicationInstallIds: ids
                    });
            },

            [[chlk.models.id.AppId, Number, String]],
            ria.async.Future, function writeReview(appId, rating, review){
                return this
                    .post('AppMarket/WriteReview.json', chlk.models.apps.AppMarketApplication, {
                        applicationId: appId.valueOf(),
                        rating: rating,
                        review: review
                    });
            }
        ])
});