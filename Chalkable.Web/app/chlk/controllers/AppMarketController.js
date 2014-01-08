REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');
REQUIRE('chlk.services.ApplicationService');

REQUIRE('chlk.activities.apps.AppMarketPage');
REQUIRE('chlk.activities.apps.AppMarketDetailsPage');
REQUIRE('chlk.activities.apps.MyAppsPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.activities.apps.InstallAppDialog');


REQUIRE('chlk.models.apps.AppMarketInstallViewData');
REQUIRE('chlk.models.apps.AppMarketDetailsViewData');
REQUIRE('chlk.models.apps.AppMarketViewData');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.apps.AppInstallPostData');
REQUIRE('chlk.models.apps.AppDeletePostData');
REQUIRE('chlk.models.apps.AppMarketPostData');
REQUIRE('chlk.models.apps.MyAppsViewData');
REQUIRE('chlk.models.apps.AppRating');
REQUIRE('chlk.models.apps.AppReviewPostData');

REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppMarketController */
    CLASS(
        'AppMarketController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AppMarketService, 'appMarketService',

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'appsService',

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'appCategoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.PictureService, 'pictureService',

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppMarketPostData]],
        function listAction(filterData_) {


            var selectedPriceType = filterData_ ? new chlk.models.apps.AppPriceType(filterData_.getPriceType())
                                                : chlk.models.apps.AppPriceType.ALL;
            var selectedSortingMode = filterData_ ? new chlk.models.apps.AppSortingMode(filterData_.getSortingMode())
                                                  : chlk.models.apps.AppSortingMode.POPULAR;


            var start = filterData_ ? filterData_.getStart() : 0;


            var selectedCategories = [];
            if (filterData_){
                selectedCategories = this.getIdsList(filterData_.getSelectedCategories(), chlk.models.id.AppCategoryId);


                selectedCategories = selectedCategories.map(function(item){
                    var category = new chlk.models.apps.AppCategory();
                    category.setId(item);
                    return category;
                });
            }

            var selectedGradeLevels = [];
            if (filterData_){
                selectedGradeLevels = this.getIdsList(filterData_.getGradeLevels(), chlk.models.id.AppGradeLevelId);
                selectedGradeLevels = selectedGradeLevels.map(function(item){
                    var gradeLvl = new chlk.models.apps.AppGradeLevel();
                    gradeLvl.setId(item);
                    return gradeLvl;
                });
            }

            var result = this.appCategoryService
                .getCategories()
                .attach(this.validateResponse_())
                .then(function(data){
                    return data.getItems();
                })
                .then(function(categories){
                    var gradeLevels = this.gradeLevelService.getGradeLevels();
                    var actualGradeLevels = selectedGradeLevels.length > 0 ? selectedGradeLevels : gradeLevels;
                    var actualCategories = selectedCategories.length > 0  ? selectedCategories : categories;

                    return this.appMarketService
                        .getApps(
                            actualCategories,
                            actualGradeLevels,
                            "",
                            selectedPriceType,
                            selectedSortingMode,
                            start
                        )
                        .attach(this.validateResponse_())
                        .then(function(apps){
                            var items = apps.getItems();

                            items = items.map(function(app){
                                var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), 640, 390);
                                app.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshots, false));
                                return app;
                            }, this);
                            apps.setItems(items);
                            return apps;

                        }, this)
                        .then(function(apps){
                            return this.appMarketService
                                .getPersonBalance(this.getCurrentPerson().getId())
                                .attach(this.validateResponse_())
                                .then(function(personBalance){
                                    return new chlk.models.apps.AppMarketViewData(
                                        apps,
                                        categories,
                                        gradeLevels,
                                        personBalance.getBalance()
                                    );
                                });
                        }, this)
                }, this)


            if (filterData_) {
                var msg = filterData_.isScroll() ? "scrollApps" : "updateApps";
                return this.UpdateView(chlk.activities.apps.AppMarketPage, result, msg);
            }
            else
                return this.PushView(chlk.activities.apps.AppMarketPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[Boolean]],
        function myAppsAction(isEdit_) {
            var isEdit = isEdit_ ? isEdit_ : false;
            var currentPersonId = this.getCurrentPerson().getId();
            var result = this.appMarketService
                .getMyApps(currentPersonId, isEdit)
                .attach(this.validateResponse_())
                .then(function(apps){
                    return new chlk.models.apps.MyAppsViewData(apps, isEdit, currentPersonId);
                });
            if (isEdit_ != null)
                return this.UpdateView(chlk.activities.apps.MyAppsPage, result);
            else
                return this.PushView(chlk.activities.apps.MyAppsPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function detailsAction(id) {
            var result = this.appMarketService
                .getDetails(id)
                .attach(this.validateResponse_())
                .then(function(app){
                    //todo: make picture dimensions constants
                    var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), 640, 390);
                    app.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshots, false));

                    var appPermissions = app.getPermissions() || [];

                    appPermissions = appPermissions.map(function(permission){
                         switch(permission.getId().valueOf()){
                             case chlk.models.apps.AppPermissionTypeEnum.MESSAGE.valueOf() : permission.setName("Messaging");
                                 break;
                             case chlk.models.apps.AppPermissionTypeEnum.GRADE.valueOf(): permission.setName("Grading");
                                 break;
                         }
                         return permission;
                    });

                    var filteredPermissions = appPermissions.filter(function(permission){
                        return permission.getId() != chlk.models.apps.AppPermissionTypeEnum.USER
                            && permission.getId() != chlk.models.apps.AppPermissionTypeEnum.ANNOUNCEMENT
                            && permission.getId() != chlk.models.apps.AppPermissionTypeEnum.CLAZZ;
                    });

                     if (appPermissions.length > filteredPermissions.length){
                         filteredPermissions.unshift(new chlk.models.apps.AppPermission(
                             chlk.models.apps.AppPermissionTypeEnum.UNKNOWN, 'Basic info'));
                     }

                    app.setPermissions(filteredPermissions);

                    var appRating = app.getApplicationRating();

                    if (!appRating)   {
                        appRating = new chlk.models.apps.AppRating();
                        appRating.setRoleRatings([]);
                        appRating.setPersonRatings([]);
                        app.setApplicationRating(appRating);
                    }

                    var installBtnTitle ='';

                    var isAlreadyInstalledForStudent = this.userInRole(chlk.models.common.RoleEnum.STUDENT) && app.isInstalledOnlyForCurrentUser();

                    if (isAlreadyInstalledForStudent){
                        installBtnTitle = 'Installed';
                    }else{
                        installBtnTitle = app.getApplicationPrice().formatPrice();
                    }
                    return this.appCategoryService
                        .getCategories()
                        .attach(this.validateResponse_())
                        .then(function(categories){
                            return this.appMarketService
                                .getPersonBalance(this.getCurrentPerson().getId())
                                .attach(this.validateResponse_())
                                .then(function(personBalance){
                                    return new chlk.models.apps.AppMarketDetailsViewData(
                                        app,
                                        installBtnTitle,
                                        categories.getItems(),
                                        this.gradeLevelService.getGradeLevels(),
                                        personBalance.getBalance(),
                                        isAlreadyInstalledForStudent
                                    );
                                }, this)
                        }, this)

                }, this);
            return this.PushView(chlk.activities.apps.AppMarketDetailsPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.appMarketService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppMarketPage, result);
        },



        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function tryToInstallAction(appId) {
            var appInfo = this.appMarketService
                .getDetails(appId)
                .attach(this.validateResponse_())
                .then(function(app){
                    var installedForGroups = app.getInstalledForGroups() || [];
                    installedForGroups.unshift(new chlk.models.apps.AppInstallGroup(
                        new chlk.models.id.AppInstallGroupId(this.getCurrentPerson().getId().valueOf()),
                        chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                        app.isInstalledOnlyForCurrentUser(),
                        "Just me"
                    ));

                    var installedCount = 0;

                    installedForGroups = installedForGroups.map(function(item){
                        if (item.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                            && this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                            item.setDescription('Whole School');
                        if (item.isInstalled()) ++installedCount;
                        return item;
                    }, this);

                    if (this.userInRole(chlk.models.common.RoleEnum.STUDENT)){
                        installedForGroups = installedForGroups.filter(function(item){
                            return item.getGroupType() != chlk.models.apps.AppInstallGroupTypeEnum.ALL;
                        });
                    }

                    app.setInstalledForGroups(installedForGroups);


                    return new chlk.models.apps.AppMarketInstallViewData(app, installedCount == installedForGroups.length);
                }, this);
            return this.ShadeView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppInstallPostData]],
        function installAction(appInstallData) {
            var totalAppPrice = this.appMarketService.getSelectedAppTotalPrice().getTotalPrice() || 0;

            return this.appMarketService
                .getPersonBalance(this.getCurrentPerson().getId())
                .attach(this.validateResponse_())
                .then(function(personBalance){
                    var userBalance = personBalance.getBalance();
                    if (totalAppPrice > userBalance){
                       return this.ShowMsgBox('You have insufficient funds to buy this app', 'Error');
                    }
                    return this.appMarketService
                        .installApp(
                            appInstallData.getAppId(),
                            this.getIdsList(appInstallData.getDepartments(), chlk.models.id.AppInstallGroupId),
                            this.getIdsList(appInstallData.getClasses(), chlk.models.id.AppInstallGroupId),
                            this.getIdsList(appInstallData.getRoles(), chlk.models.id.AppInstallGroupId),
                            this.getIdsList(appInstallData.getGradeLevels(), chlk.models.id.AppInstallGroupId),
                            appInstallData.getCurrentPerson()
                        )
                        .attach(this.validateResponse_())
                        .then(function(result){
                            var title = result ? "Installation successful" : "Error while installing app.";
                               return this.ShowMsgBox(title, '', [{
                                   text: 'Ok',
                                   controller: 'appmarket',
                                   action: 'myApps',
                                   params: [],
                                   color: chlk.models.common.ButtonColor.GREEN.valueOf()
                               }], 'center');
                        }, this);
                }, this);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[String]],
        function uninstallAction(ids) {
            return this.appMarketService
                .uninstallApps(ids)
                .attach(this.validateResponse_())
                .then(function(result){
                    return this.Redirect('appmarket', 'myApps', [false]);
                }, this);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppDeletePostData]],
        function tryToUninstallAction(data){
            if(!data.isUninstallable()){
                this.ShowMsgBox(
                    'Sorry you can\'t uninstall app. You are not owner of this app.',
                    'Oops.', [{
                        text: 'Ok',
                        color: chlk.models.common.ButtonColor.GREEN.valueOf()
                    }]
                );
            }
            else{
                var additionalMsg = ''
                if(!data.isSelfInstalled()){
                    additionalMsg = 'NOTE: This app is not self installed.';
                }
                var msgTitle = 'Uninstall the ' + data.getAppName() + ' app?' + additionalMsg;

                return this.ShowMsgBox(msgTitle, 'just checking.', [{
                    text: 'CANCEL',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf()
                }, {
                    text: 'UNINSTALL',
                    controller: 'appmarket',
                    action: 'uninstall',
                    params: [data.getApplicationInstallIds()],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                }], 'center');

            }
        },

        [[chlk.models.apps.AppReviewPostData]],
        function writeReviewAction(reviewData){
            var result = this.appMarketService
                .writeReview(
                    reviewData.getAppId(),
                    reviewData.getRating(),
                    reviewData.getReview()
                )
                .attach(this.validateResponse_())
                .then(function(data){
                   var appRating = data.getApplicationRating();
                    if (!appRating)   {
                        appRating = new chlk.models.apps.AppRating();
                        appRating.setRoleRatings([]);
                        appRating.setPersonRatings([]);
                        data.setApplicationRating(appRating);
                    }
                    return data;
                });
            return this.UpdateView(chlk.activities.apps.AppMarketDetailsPage, result, 'updateReviews');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.ADMINGRADE,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AppId]],
        function banAppAction(appId){
            var result = this.appsService
                .banApp(appId)
                .attach(this.validateResponse_())
                .then(function(res){
                  return res;
                });
            return this.UpdateView(chlk.activities.apps.AppMarketDetailsPage, result, 'banApp');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.ADMINGRADE,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AppId]],
        function unbanAppAction(appId){
            var result = this.appsService
                .unbanApp(appId)
                .attach(this.validateResponse_())
                .then(function(res){
                    return res;
                });
            return this.UpdateView(chlk.activities.apps.AppMarketDetailsPage, result, 'banApp');
        }
    ])
});
