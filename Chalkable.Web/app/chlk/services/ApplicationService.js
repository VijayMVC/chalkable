REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('ria.async.Observable');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppPlatform');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.AppPersonRating');
REQUIRE('chlk.models.apps.ApplicationAuthorization');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppPermissionId');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DevApplicationListChangeEvent */
    DELEGATE(
        [[ArrayOf(chlk.models.apps.Application)]],
        VOID, function DevApplicationListChangeEvent(list) {});

    /** @class chlk.services.ApplicationService */
    CLASS(
        'ApplicationService', EXTENDS(chlk.services.BaseService), [

            READONLY, ria.async.IObservable, 'devApplicationListChange',

            function $() {
                BASE();
                this.devApplicationListChange = new ria.async.Observable(chlk.services.DevApplicationListChangeEvent);
            },

            [[Number, chlk.models.id.SchoolPersonId, Number, String]],
            ria.async.Future, function getApps(pageIndex_, developerId_, state_, filter_) {
                return this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {
                        start: pageIndex_,
                        developerId: developerId_ ? developerId_.valueOf() : null,
                        state: state_ || null,
                        filter: filter_,
                        count: 25
                    })
            },

            [[Boolean]],
            ria.async.Future, function getDevApps(refresh_) {
                var apps = this.getContext().getSession().get(ChlkSessionConstants.DEV_APPS) || [];

                return apps.length == 0 || refresh_
                    ? this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {
                            state: chlk.models.apps.AppStateEnum.DRAFT.valueOf()
                        })
                          .then(function(data){
                                this.getContext().getSession().set(ChlkSessionConstants.DEV_APPS, data.getItems());
                                return data.getItems();
                    }, this)
                    : new ria.async.DeferredData(apps);
            },

            [[chlk.models.apps.Application]],
            function switchApp(app){
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_APP_ID, app.getId());
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_APP, app);
                return app;
            },

            function getCurrentAppId(){
                return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_APP_ID);
            },

            function getCurrentApp(){
                return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_APP);
            },

            [[chlk.models.id.AppId, Boolean]],
            ria.async.Future, function getInfo(appId_, switchApp_) {
                var mustSwitch = switchApp_ ? switchApp_ : true;
                var appId_ = appId_ ? appId_ : this.getCurrentAppId();

                return appId_ ? this
                                    .get('Application/GetInfo.json', chlk.models.apps.Application, {applicationId: appId_.valueOf()})
                                    .then(function(app){
                                            return new ria.async.DeferredData(mustSwitch ? this.switchApp(app) : app);
                                    }, this)
                              : ria.async.DeferredData(new chlk.models.apps.Application());
            },

            [[Object, Number, Number]],
            ria.async.Future, function uploadPicture(file, width_, height_) {

                //switched width and height intentionally
                return this.uploadFiles('Application/UploadPicture', file, chlk.models.id.PictureId, {
                    width: height_,
                    height: width_
                });
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function installApp(appId) {
                return this
                    .post('Application/Install.json', Boolean, {applicationId: appId.valueOf()});
            },

            [[chlk.models.id.SchoolPersonId]],
            function testDevApps(devId) {
                window.location.href = "/DemoSchool/TestApps.json?prefix=" + devId.valueOf();
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function approveApp(appId) {
                    return this
                        .post('Application/Approve.json', Boolean, {applicationId: appId.valueOf()});
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function declineApp(appId) {
                return this
                    .post('Application/Decline.json', Boolean, {applicationId: appId.valueOf()});
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function goLive(appId) {
                return this
                    .post('Application/GoLive.json', Boolean, {applicationId: appId.valueOf()})
                    .then(function(data){
                        var currentAppId = this.getCurrentAppId();
                        return this.getInfo(currentAppId);
                    }, this)
            },


            [[chlk.models.id.AppId]],
            ria.async.Future, function unlist(appId) {
                return this
                    .post('Application/Unlist.json', Boolean, {applicationId: appId.valueOf()});

            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.AppId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function addToAnnouncement(personId, appId, announcementId, announcementType) {
                return this
                    .post('Application/AddToAnnouncement.json', chlk.models.apps.AppAttachment, {
                        applicationId: appId.valueOf(),
                        announcementId: announcementId.valueOf(),
                        announcementType: announcementType.valueOf()
                    })
                    .then(function(attachment){
                        return this.getOauthCode(personId, attachment.getUrl())
                            .then(function(data){
                                attachment.setOauthCode(data.getAuthorizationCode());
                                return attachment;
                            });
                    }, this);
            },

            [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementType]],
            ria.async.Future, function attachApp(appAnnouncementId, announcementType_) {
              return this
                  .post('Application/Attach.json', chlk.models.announcement.FeedAnnouncementViewData, {
                      announcementApplicationId: appAnnouncementId.valueOf(),
                      announcementType: announcementType_ && announcementType_.valueOf()
                  });
            },

            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function getOauthCode(personId, appUrl){
                var forEdit = false;
                return this.get('Application/GetOauthCode.json', chlk.models.apps.ApplicationAuthorization, {
                    applicationUrl: appUrl
                }).transform(function (applicationAuthorization) {
                    var app = applicationAuthorization.getApplication();
                    var appInstalls = app.getApplicationInstalls() || [];
                    app.setSelfInstalled(false);
                    var uninstallAppIds = [];

                    appInstalls.forEach(function(appInstall){
                        if (appInstall.isOwner() && forEdit){
                            uninstallAppIds.push(appInstall.getAppInstallId());
                            app.setSelfInstalled(appInstall.getPersonId() == appInstall.getInstallationOwnerId());
                        }
                        app.setPersonal(appInstall.getPersonId() == personId);
                    });
                    app.setUninstallable(forEdit && uninstallAppIds.length > 0);
                    var ids = uninstallAppIds.map(function(item){
                        return item.valueOf()
                    }).join(',');
                    app.setApplicationInstallIds(ids);

                    return applicationAuthorization;
                });
            },

            [[chlk.models.id.SchoolPersonId, String]],
            ria.async.Future, function createApp(devId, name) {
                return this
                    .post('Application/Create.json', chlk.models.apps.Application, {
                        developerId: devId.valueOf(),
                        name: name
                    })
                    .then(function (data) {
                        return this.getDevApps(true)
                        .then(function(items){
                            this.devApplicationListChange.notify([items]);
                            return this.switchApp(data);
                        }, this)
                    }, this);
            },
            [[chlk.models.id.AppId]],
            ria.async.Future, function deleteApp(appId) {
                return this
                    .post('Application/Delete.json', Boolean, {applicationId: appId.valueOf()})
                    .then(function (data) {
                        return this.getDevApps(true)
                            .then(function(items){
                                this.devApplicationListChange.notify([items]);
                                return this.switchApp(items.length > 0 ? items[items.length - 1] : new chlk.models.apps.Application());
                            }, this)
                    }, this);
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function sysAdminDeleteApp(appId) {
                return this
                    .post('Application/Delete.json', Boolean, {applicationId: appId.valueOf()});
            },

            [[
                chlk.models.id.AppId,
                chlk.models.apps.ShortAppInfo,
                ArrayOf(chlk.models.apps.AppPermissionTypeEnum),
                chlk.models.apps.AppPrice,
                chlk.models.id.SchoolPersonId,
                chlk.models.apps.AppAccess,
                ArrayOf(chlk.models.id.AppCategoryId),
                ArrayOf(chlk.models.id.PictureId),
                ArrayOf(chlk.models.id.GradeLevelId),
                ArrayOf(chlk.models.apps.AppPlatformTypeEnum),
                Boolean,
                ArrayOf(chlk.models.id.CommonCoreStandardId)
            ]],
            ria.async.Future, function updateApp(
                appId, shortAppInfo, permissionIds, appPricesInfo, devId, appAccess, categories, pictures_,
                gradeLevels, platforms, forSubmit, standards){
                return this.post('Application/Update.json', chlk.models.apps.Application,  {
                    applicationId: appId.valueOf(),
                    shortApplicationInfo: shortAppInfo.getPostData(),
                    permissions: this.arrayToIds(permissionIds),
                    applicationPrices: appPricesInfo.getPostData(),
                    developerId: devId.valueOf(),
                    applicationAccessInfo: appAccess.getPostData(),
                    categories: this.arrayToIds(categories),
                    picturesid: this.arrayToIds(pictures_ || ""),
                    gradeLevels: this.arrayToIds(gradeLevels),
                    platforms: this.arrayToIds(platforms),
                    forSubmit: forSubmit,
                    standardsIds : this.arrayToIds(standards)
                })
                .then(function(newApp){
                    return this.switchApp(newApp);
                }, this);
                //here only if name was changed
            },


            ArrayOf(chlk.models.apps.AppPermission), function getAppPermissions(){
                return  [
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.USER, "User"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.MESSAGE, "Message"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.GRADE, "Grade"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.ATTENDANCE, "Attendance"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.ANNOUNCEMENT, "Announcement"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.CLAZZ, "Class"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.SCHEDULE, "Schedule"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.DISCIPLINE, "Discipline"),
                    new chlk.models.apps.AppPermission(chlk.models.apps.AppPermissionTypeEnum.PRACTICE, "Practice")
                ];
            },

            ArrayOf(chlk.models.apps.AppPlatform), function getAppPlatforms(){
                return  [
                    new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.WEB, "Web"),
                    new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.IOS, "iOS"),
                    new chlk.models.apps.AppPlatform(chlk.models.apps.AppPlatformTypeEnum.ANDROID, "Android")
                ];
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function banApp(appId){
                return this
                    .post('Application/BanApp.json', null, {
                        applicationId: appId.valueOf()
                    });
            },

            [[chlk.models.id.AppId]],
            ria.async.Future, function unbanApp(appId){
                return this
                    .post('Application/UnbanApp.json', null, {
                        applicationId: appId.valueOf()
                    });
            },

            [[chlk.models.id.AppId, Number]],
            ria.async.Future, function getAppReviews(appId, start_){
                return this
                    .get('Application/GetAppReviews.json', chlk.models.apps.AppRating, {
                        applicationId: appId.valueOf(),
                        start: start_ || 0
                    });
            },

            ria.async.Future, function getAppAnalytics(appId){
                return this
                    .post('Application/GetAppAnalytics.json', chlk.models.developer.HomeAnalytics, {
                        applicationId: appId.valueOf()
                    });
            },

            [[chlk.models.id.AppId, Boolean]],
            ria.async.Future, function changeAppType(appId, isInternal){
                return this
                    .post('Application/ChangeApplicationType.json', chlk.models.Success, {
                        isInternal: isInternal,
                        applicationId: appId.valueOf()
                    });
            },

            [[chlk.models.id.AppId, Number, String]],
            ria.async.Future, function setInternalData(appId, internalScore, internalDescription){
                return this.post('Application/SetApplicationInternalData.json', chlk.models.Success,{
                    applicationId: appId.valueOf(),
                    internalScore: internalScore,
                    internalDescription: internalDescription
                });
            }

        ])
});