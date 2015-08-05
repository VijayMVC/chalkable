REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.apps.AppMarketPage');
REQUIRE('chlk.activities.apps.AppMarketDetailsPage');
REQUIRE('chlk.activities.apps.MyAppsPage');
REQUIRE('chlk.activities.apps.AttachDialog');
REQUIRE('chlk.activities.apps.InstallAppDialog');
REQUIRE('chlk.activities.apps.QuickAppInstallDialog');


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
REQUIRE('chlk.models.apps.ShortAppInstallViewData');
REQUIRE('chlk.models.common.RequestResultViewData');

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

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
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

                    var res = ria.async.wait([
                        this.appMarketService.getPersonBalance(this.getCurrentPerson().getId()),
                        this.appMarketService
                            .getApps(
                            actualCategories,
                            actualGradeLevels,
                            "",
                            selectedPriceType,
                            selectedSortingMode,
                            start
                        )
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var apps = result[1];
                        var balance = result[0].getBalance();
                        var items = apps.getItems();

                        var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();
                        items = items.map(function(app){
                            var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), screenshotDims.width, screenshotDims.height);
                            app.setScreenshotPictures(new chlk.models.apps.AppScreenShots(screenshots, false));
                            return app;
                        }, this);
                        apps.setItems(items);

                        return new chlk.models.apps.AppMarketViewData(
                            apps,
                            categories,
                            gradeLevels,
                            balance
                        );
                    }, this);

                    return res;
                }, this);

            if (filterData_) {
                var msg = filterData_.isScroll() ? "scrollApps" : "updateApps";
                return this.UpdateView(chlk.activities.apps.AppMarketPage, result, msg);
            }
            else
                return this.PushView(chlk.activities.apps.AppMarketPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[Boolean]],
        function myAppsAction(isEdit_) {
            var isEdit = isEdit_ ? isEdit_ : false;
            var currentPersonId = this.getCurrentPerson().getId();
            var result = this.appMarketService
                .getMyApps(currentPersonId, isEdit, 0, null, 10000)
                .attach(this.validateResponse_())
                .then(function(apps){
                    return new chlk.models.apps.MyAppsViewData(apps, isEdit, currentPersonId);
                });
            if (isEdit_ != null)
                return this.UpdateView(chlk.activities.apps.MyAppsPage, result);
            else
                return this.PushView(chlk.activities.apps.MyAppsPage, result);
        },


        [[chlk.models.apps.AppMarketApplication]],
        chlk.models.apps.AppMarketApplication, function prepareAppPictures_(app) {
            var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();
            var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), screenshotDims.width, screenshotDims.height);
            app.setScreenshotPictures(new chlk.models.apps.AppScreenShots(screenshots, false));
            return app;
        },

        //todo: refactor
        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AppId, Boolean]],
        function detailsAction(id, fromNewItem_) {
            var result = this.appMarketService
                .getDetails(id)
                .attach(this.validateResponse_())
                .then(function(app){
                    app = this.prepareAppPictures_(app);
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

                    var result = ria.async.wait([
                        this.appCategoryService.getCategories(),
                        this.appMarketService.getPersonBalance(this.getCurrentPerson().getId())
                    ])
                    .attach(this.validateResponse_())
                    .then(function(res){
                        var categories = res[0].getItems();
                        var balance = res[1].getBalance();
                        return new chlk.models.apps.AppMarketDetailsViewData(
                            app,
                            installBtnTitle,
                            categories,
                            this.gradeLevelService.getGradeLevels(),
                            balance,
                            isAlreadyInstalledForStudent,
                            fromNewItem_
                        );
                    },this);
                    return result;
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

        [[chlk.models.id.ClassId, String, String]],
        function getSuggestedAppsAction(classId, academicBenchmarkIds, standardUrlComponents_) {
            var result = this.appMarketService
                .getSuggestedApps(classId, null, academicBenchmarkIds, null, null, true)
                .attach(this.validateResponse_())
                .then(function(apps){
                    return new chlk.models.apps.SuggestedAppsList(classId, null, apps, null, standardUrlComponents_)
                });
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'apps');
        },


        [[chlk.models.apps.AppMarketApplication]],
        chlk.models.apps.AppMarketApplication, function prepareApplicationInstallGroups_(app){
            var installedForGroups = app.getInstalledForGroups() || [];
            installedForGroups.unshift(new chlk.models.apps.AppInstallGroup(
                new chlk.models.id.AppInstallGroupId(this.getCurrentPerson().getId().valueOf()),
                chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                app.isInstalledOnlyForCurrentUser(), Msg.Just_me
            ));
            var installedCount = 0;
            installedForGroups = installedForGroups.map(function(item){
                if (item.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.ALL){
                    item.setId(new chlk.models.id.AppInstallGroupId('all'));
                }

                if (item.isInstalled()) ++installedCount;

                if (item.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ){
                    var cls = this.classService.getClassById(new chlk.models.id.ClassId(item.getId().valueOf()));
                    var classNumber = cls && cls.getClassNumber() ? cls.getClassNumber()+ " " : "";
                    item.setTooltipHint(classNumber + item.getDescription());
                }
                return item;
            }, this);

            if (this.userInRole(chlk.models.common.RoleEnum.STUDENT)){
                installedForGroups = installedForGroups.filter(function(item){
                    return item.getGroupType() != chlk.models.apps.AppInstallGroupTypeEnum.ALL;
                });
            }

            app.setInstalledForGroups(installedForGroups);
            app.setAlreadyInstalled(installedCount == installedForGroups.length);
            return app;
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AppId, Boolean]],
        function tryToInstallAction(appId, fromNewItem_) {
            var appInfo = this.appMarketService
                .getDetails(appId)
                .attach(this.validateResponse_())
                .then(function(app){
                    app = this.prepareAppPictures_(app);
                    app = this.prepareApplicationInstallGroups_(app);
                    return new chlk.models.apps.AppMarketInstallViewData(app, null, fromNewItem_);
                }, this);
            return this.ShadeOrUpdateView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AppId]],
        function tryToInstallDistrictAdminAction(appId, fromNewItem_) {
                var appInfo = ria.async.wait(
                    this.appMarketService.getDetails(appId).attach(this.validateResponse_()),
                    this.classService.getAllSchoolsActiveClasses().attach(this.validateResponse_())
                )
                .then(function(data){
                    var app = this.prepareAppPictures_(data[0]);
                    app = this.prepareApplicationInstallGroups_(app);
                    return new chlk.models.apps.AppMarketInstallViewData(app, data[1], fromNewItem_);
                }, this);
            return this.ShadeOrUpdateView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AppId, chlk.models.apps.AppTotalPrice, chlk.models.id.ClassId, chlk.models.id.AnnouncementId]],
        function tryToQuickInstallAction(applicationId, appTotalPrice, classId, announcementId_){
            var res = this.appMarketService
                .getDetails(applicationId)
                .attach(this.validateResponse_())
                .then(function(app){
                    var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();
                    var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), screenshotDims.width, screenshotDims.height);
                    app.setScreenshotPictures(new chlk.models.apps.AppScreenShots(screenshots, false));
                    return new chlk.models.apps.ShortAppInstallViewData(app, appTotalPrice, classId, announcementId_);
                }, this);
            return this.ShadeView(chlk.activities.apps.QuickAppInstallDialog, res);
        },


        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId]],
        function quickAppAttachAction(announcementId, applicationId){
            this.BackgroundCloseView(chlk.activities.apps.QuickAppInstallDialog);
            return this.Redirect('apps', 'tryToAttach', [announcementId, applicationId]);
        },


        [chlk.controllers.SidebarButton('apps')],
        [[Boolean, chlk.models.id.AppId]],
        function installCompleteAction(result, appId) {
            var title = result ? "Installation successful" : "Error while installing app.";
            return this.ShowMsgBox(title, '', [{
                text: 'Ok',
                controller: 'appmarket',
                action: 'myApps',
                params: [],
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }], 'center'), null;
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.AppInstallPostData]],
        function installFailAction(){
            return this.ShowMsgBox('You have insufficient funds to buy this app', 'Error'), null;
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.SchoolPersonId, Boolean, chlk.models.id.AppId]],
        function quickInstallCompleteAction(personId, result, appId){
            var res = this.appMarketService.getPersonBalance(personId, true)
                .attach(this.validateResponse_())
                .then(function(data){
                    var text = 'App succesfully installed! Your new balance is ' + data.getBalance();
                    this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementFormPage, chlk.models.common.SimpleObject(appId.valueOf()), chlk.activities.lib.DontShowLoader());
                    this.BackgroundUpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, chlk.models.common.SimpleObject(appId.valueOf()), chlk.activities.lib.DontShowLoader());
                    this.BackgroundUpdateView(chlk.activities.announcement.LessonPlanFormPage, chlk.models.common.SimpleObject(appId.valueOf()), chlk.activities.lib.DontShowLoader());
                    return new chlk.models.common.RequestResultViewData(true, text);
                }, this);

            return this.UpdateView(chlk.activities.apps.QuickAppInstallDialog, res);
        },

        [chlk.controllers.SidebarButton('apps')],
        function quickInstallFailAction(){
            //todo: show dialog for adding funds
            return this.ShowMsgBox('You have insufficient funds to buy this app', 'Error')
                .then(function(){
                    this.BackgroundCloseView(chlk.activities.apps.QuickAppInstallDialog);
                }, this), null;
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.apps.AppInstallPostData]],
        function quickInstallAction(appInstallData){
            var res = this.install_(appInstallData, 'quickInstallComplete'
                , [this.getCurrentPerson().getId()]
                , 'quickInstallFail', null);
            return this.UpdateView(chlk.activities.apps.QuickAppInstallDialog, res);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[Object]],
        function updateGroupsAction(data) {
            this.BackgroundCloseView(chlk.activities.announcement.AnnouncementGroupsDialog);
            this.BackgroundNavigate('appmarket', 'tryToInstall', [data.id]);
            return null;
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.apps.AppInstallPostData]],
        function installAction(appInstallData) {
            switch(appInstallData.getSubmitActionType()){
                case 'install':
                    var res = this.install_(appInstallData, 'installComplete', null, 'installFail', null);
                    if(appInstallData.isFromNewItem())
                        return this.Redirect('announcement', 'add');
                    return this.UpdateView(chlk.activities.apps.InstallAppDialog, res);

                case 'getAppPrice':
                    var appInstallArgs = this.prepareAppTotalPriceCallParams_(appInstallData);

                    var res;
                    if (!appInstallData.isEmpty())
                        res = this.appMarketService.getApplicationTotalPrice.apply(null, appInstallArgs);
                    else
                        res = new ria.async.DeferredData(new chlk.models.apps.AppTotalPrice.$createEmpty());
                    return this.UpdateView(chlk.activities.apps.InstallAppDialog, res, 'getAppPrice');

                default:
                    _DEBUG && console.warn('Unsupported submit-action-type', appInstallData.getSubmitActionType());
                    return null;
            }
        },

        [[chlk.models.apps.AppInstallPostData]],
        Array, function prepareAppTotalPriceCallParams_(appInstallData){
            return [
                appInstallData.getAppId(),
                this.getIdsList(appInstallData.getDepartments(), chlk.models.id.AppInstallGroupId),
                this.getIdsList(appInstallData.getClasses(), chlk.models.id.AppInstallGroupId),
                this.getIdsList(appInstallData.getRoles(), chlk.models.id.AppInstallGroupId),
                this.getIdsList(appInstallData.getGradeLevels(), chlk.models.id.AppInstallGroupId),
                this.getIdsList(appInstallData.getGroups(), chlk.models.id.AppInstallGroupId),
                appInstallData.getCurrentPerson()
            ];
        },

        [[chlk.models.apps.AppInstallPostData, String, Array, String, Array]],
        function install_(appInstallData, installCompleteAction, completeActionArgs, installFailAction, failActionArgs){

            var appInstallArgs = this.prepareAppTotalPriceCallParams_(appInstallData);
            var res = ria.async.wait(
                this.appMarketService.getApplicationTotalPrice.apply(null, appInstallArgs).attach(this.validateResponse_()),
                this.appMarketService.getPersonBalance(this.getCurrentPerson().getId()).attach(this.validateResponse_())
            )
                .then(function(res){
                    var totalAppPrice = res[0].getTotalPrice() || 0;
                    var personBalance = res[1];
                    var userBalance = personBalance.getBalance();
                    if(res[0].getTotalPersonsCount() <= 0){
                        return this.ShowMsgBox('This app is already installed.', 'Error')
                            .then(function(){
                                this.BackgroundCloseView(chlk.activities.apps.QuickAppInstallDialog);
                                this.BackgroundCloseView(chlk.activities.apps.InstallAppDialog);
                                return ria.async.BREAK;
                            }, this);
                    }
                    if (totalAppPrice > userBalance){
                        return this.BackgroundNavigate('appmarket', installFailAction, (failActionArgs || []))
                            , ria.async.BREAK;
                    }

                    return null;
                }, this)
                .thenCall(this.appMarketService.installApp, appInstallArgs)
                .attach(this.validateResponse_())
                .then(function(result){
                    completeActionArgs = completeActionArgs || [];
                    completeActionArgs.push(result);
                    completeActionArgs.push(appInstallArgs[0]);
                    return this.BackgroundNavigate('appmarket', installCompleteAction, completeActionArgs);
                }, this);

            return res;
        },

        [chlk.controllers.SidebarButton('apps')],
        [[String]],
        function uninstallAction(ids) {
            this.appMarketService
                .uninstallApps(ids)
                .attach(this.validateResponse_())
                .then(function(result){
                    return this.BackgroundNavigate('appmarket', 'myApps');
                    //return this.Redirect('appmarket', 'myApps', [false]);
                }, this);
            return this.ShadeLoader();
        },

        function cancelAppDeleteAction(){
            var result = new ria.async.DeferredData(new chlk.models.Success());
            return this.UpdateView(chlk.activities.apps.MyAppsPage, result, chlk.activities.lib.DontShowLoader());
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
                return null;
            }
            else{
                var additionalMsg = '';
                var warningMsg = '<div class="msg-detail">Uninstalling an app removes it for you and anyone you installed it for.</div>';
                if(!data.isSelfInstalled()){
                    additionalMsg = '<div class="msg-detail">NOTE: This app is not self installed.</div>';
                    warningMsg = '';
                }

                var msgTitle = warningMsg + '<div class="msg-action">Uninstall the ' + data.getAppName() + ' app?</div>' + additionalMsg;

                return this.ShowMsgBox(msgTitle, 'just checking.', [{
                    text: 'CANCEL',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    controller: 'appmarket',
                    action: 'cancelAppDelete'
                }, {
                    text: 'UNINSTALL',
                    controller: 'appmarket',
                    action: 'uninstall',
                    params: [data.getApplicationInstallIds()],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                }], 'delete-app', true), null;

            }
            return this.ShadeLoader();
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
            var result = this.ShowConfirmBox('Banning this App will hide the app in the store for all users in your district. If the app was already installed by a user, it will not be uninstalled.', null, 'Ban', 'negative-button')
                .thenCall(this.appsService.banApp, [appId])
                .attach(this.validateResponse_())
                .then(function(data) {
                    var res = new chlk.models.apps.AppMarketApplication;
                    res.setId(appId);
                    res.setBanned(true);
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
            var result = this.ShowConfirmBox('Un ban this app?', null, 'Yes')
                .thenCall(this.appsService.unbanApp, [appId])
                .attach(this.validateResponse_())
                .then(function(data){
                    var res = new chlk.models.apps.AppMarketApplication;
                    res.setId(appId);
                    res.setBanned(false);
                    return res;
                });
            return this.UpdateView(chlk.activities.apps.AppMarketDetailsPage, result, 'banApp');
        }
    ])
});
