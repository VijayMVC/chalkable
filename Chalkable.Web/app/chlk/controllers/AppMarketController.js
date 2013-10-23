REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');

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
        chlk.services.AppCategoryService, 'appCategoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.PictureService, 'pictureService',

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppMarketPostData]],
            //todo: add sort mode and other options
        function listAction(filterData_) {


            var selectedPriceType = filterData_ ? new chlk.models.apps.AppPriceType(filterData_.getPriceType())
                                                : chlk.models.apps.AppPriceType.ALL;
            var selectedSortingMode = filterData_ ? new chlk.models.apps.AppSortingMode(filterData_.getSortingMode())
                                                  : chlk.models.apps.AppSortingMode.POPULAR;


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

            //todo: sort if there is filterdata
            var result = this.appCategoryService
                .getCategories()
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
                            selectedSortingMode
                        )
                        .then(function(apps){
                            var items = apps.getItems();
                            items = items.map(function(app){
                                var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), 640, 390);
                                app.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshots, false));
                                return app;
                            }, this);
                            apps.setItems(items);
                            //TODO: get current balance
                            return new chlk.models.apps.AppMarketViewData(apps, categories, gradeLevels, 0);
                        }, this)
                }, this)
                .attach(this.validateResponse_());

            if (filterData_)
                return this.UpdateView(chlk.activities.apps.AppMarketPage, result, 'updateApps');
            else
                return this.PushView(chlk.activities.apps.AppMarketPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        function myAppsAction() {
            var result = this.appMarketService
                .getMyApps(this.getCurrentPerson().getId(), false)
                .then(function(apps){
                    return new chlk.models.apps.MyAppsViewData(apps, false);
                })
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.MyAppsPage, result);
        },



        [chlk.controllers.SidebarButton('apps')],
        [[Boolean]],
        function updateMyAppsAction(isEdit) {
            var result = this.appMarketService
                .getMyApps(this.getCurrentPerson().getId(), isEdit)
                .then(function(data){
                    return new chlk.models.apps.MyAppsViewData(data, isEdit);
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.MyAppsPage, result);
        },


        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function detailsAction(id) {
            var result = this.appMarketService
                .getDetails(id)
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


                    //todo: when ready on server remove this


                    var appRating = app.getApplicationRating();


                    if (!appRating)   {
                        appRating = new chlk.models.apps.AppRating();
                        appRating.setRoleRatings([]);
                        appRating.setPersonRatings([]);
                        app.setApplicationRating(appRating);
                    }


                    /*appRating.setAverage(5);

                    var personRating = new chlk.models.apps.PersonRating();
                    personRating.setReview("Great app");
                    personRating.setRoleName("Teacher");
                    personRating.setRoleId(4);
                    personRating.setRating(5);

                    var roleRating = new chlk.models.apps.RoleRating();
                    roleRating.setRating(3);
                    roleRating.setRoleName("Teacher");
                    roleRating.setRoleId(4);
                    roleRating.setPersonCount(5);

                    appRating.setPersonRatings([personRating]);
                    appRating.setRoleRatings([roleRating]);

                    app.setApplicationRating(appRating);*/

                    var installBtnTitle ='';

                    if (this.userInRole(chlk.models.common.RoleEnum.STUDENT) && app.isInstalledOnlyForCurrentUser()){
                        installBtnTitle = 'Installed';
                    }else{
                        installBtnTitle = app.getApplicationPrice().formatPrice();
                    }

                    /*
                     if(data.videodemourl){
                         max++;
                         var ifrm = document.createElement("embed");
                         ifrm.setAttribute("src", getVideoUrl(data.videodemourl));
                         ifrm.setAttribute("id", "app-img");
                         ifrm.setAttribute("wmode", "transparent");
                         ifrm.setAttribute("width", "100%");
                         ifrm.setAttribute("height", "387px");
                         images.push(ifrm);
                     }

                     */
                    return this.appCategoryService
                        .getCategories()
                        .then(function(categories){
                            return new chlk.models.apps.AppMarketDetailsViewData(
                                app,
                                installBtnTitle,
                                categories.getItems(),
                                this.gradeLevelService.getGradeLevels(),
                                0
                                ); //todo: add real balance
                        }, this)

                }, this)
                .attach(this.validateResponse_());
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
                .then(function(app){
                    var installedForGroups = app.getInstalledForGroups() || [];
                    if (!this.userInRole(chlk.models.common.RoleEnum.STUDENT)){
                        installedForGroups.unshift(new chlk.models.apps.AppInstallGroup(
                            new chlk.models.id.AppInstallGroupId(this.getCurrentPerson().getId().valueOf()),
                            chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                            app.isInstalledOnlyForCurrentUser(),
                            "Just me"
                        ));
                    }

                    var installedCount = 0;

                    installedForGroups = installedForGroups.map(function(item){
                        if (item.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                            && this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                            item.setDescription('Whole School');
                        if (item.isInstalled()) ++installedCount;
                        return item;
                    }, this);
                    app.setInstalledForGroups(installedForGroups);


                    return new chlk.models.apps.AppMarketInstallViewData(app, installedCount == installedForGroups.length);
                }, this)
                .attach(this.validateResponse_())

            return this.ShadeView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppInstallPostData]],
        function installAction(appInstallData) {
            return this.appMarketService
                .installApp(
                    appInstallData.getAppId(),
                    this.getIdsList(appInstallData.getDepartments(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getClasses(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getRoles(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getGradeLevels(), chlk.models.id.AppInstallGroupId),
                    appInstallData.getCurrentPerson()
                )
                .then(function(result){
                    var title = result ? "Installation successful" : "Error while installing app.";
                       return this.ShowMsgBox(title, '', [{
                           text: 'Ok',
                           controller: 'appmarket',
                           action: 'list',
                           params: [],
                           color: chlk.models.common.ButtonColor.GREEN.valueOf()
                       }], 'center');
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.SidebarButton('apps')],
        [[String]],
        function uninstallAction(ids) {
            return this.appMarketService
                .uninstallApps(ids)
                .then(function(result){
                    return this.Redirect('appmarket', 'updateMyApps', [false]);
                }, this)
                .attach(this.validateResponse_());
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
                ).then(function(data){
                   var appRating = data.getApplicationRating();
                    if (!appRating)   {
                        appRating = new chlk.models.apps.AppRating();
                        appRating.setRoleRatings([]);
                        appRating.setPersonRatings([]);
                        data.setApplicationRating(appRating);
                    }
                    return data;
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppMarketDetailsPage, result, 'updateReviews');
        }

    ])
});
