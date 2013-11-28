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

REQUIRE('chlk.models.apps.AppPriceType');
REQUIRE('chlk.models.apps.AppSortingMode');
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
                        count: count_,
                        categoriesIds:  categoryIds,
                        gradeLevelsIds: gradeLvlsIds,
                        filter: filter,
                        filterMode: filterMode_ && filterMode_.valueOf(),
                        sortingMode: sortingMode_ && sortingMode_.valueOf()
                    });
            },

            [[chlk.models.id.SchoolPersonId, Number, String]],
            ria.async.Future, function getInstalledApps(personId, start_, filter_) {
                return this
                    .getPaginatedList('AppMarket/ListInstalled.json', chlk.models.apps.AppMarketApplication, {
                        personId: personId.valueOf(),
                        start: start_ | 0,
                        filter: filter_ || ""
                    })
            },

            [[chlk.models.id.SchoolPersonId, Boolean, Number, String]],
            ria.async.Future, function getMyApps(personId, forEdit, start_, filter_) {
                return this
                    .getInstalledApps(personId, start_, filter_)
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

                        this.getContext().getSession().set('myAppsCached', apps);
                        return data;
                    }, this);
            },

            [[String]],
            ria.async.Future, function getMyAppsByFilter(filter) {
                var personId = this.getContext().getSession().get('currentPerson').getId();
                return this.getMyApps(personId, false, 0, filter);
            },




            [[String]],
            chlk.models.apps.AppMarketApplication, function getMyAppByUrl(url){
                var myApps = this.getContext().getSession().get('myAppsCached') || [];
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
                        this.getPersonBalance(this.getContext().getSession().get('currentPerson').getId(), true);
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
                    })
                    .then(function(totalPrice){
                        this.getContext().getSession().set('selectedAppTotalPrice', totalPrice);
                        return totalPrice;
                    }, this);
            },

            chlk.models.apps.AppTotalPrice, function getSelectedAppTotalPrice(){
                return this.getContext().getSession().get('selectedAppTotalPrice', new chlk.models.apps.AppTotalPrice);
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getPersonBalance(personId, refresh_) {
                var currentBalance = this.getContext().getSession().get('currentPersonBalance');
                return currentBalance == null || refresh_ ?
                    this.get('Fund/GetPersonBudgetBalance.json', chlk.models.funds.PersonBalance, {
                        personId: personId.valueOf()
                    })
                    .then(function(balance){
                        this.getContext().getSession().set('currentPersonBalance', balance);
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