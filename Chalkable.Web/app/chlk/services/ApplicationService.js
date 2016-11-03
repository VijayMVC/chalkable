REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('ria.async.Observable');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPermission');
REQUIRE('chlk.models.apps.AppPlatform');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.ApplicationAuthorization');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.apps.ApplicationContent');

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

            ria.async.Future, function getListForPanorama() {
                return this.get('Application/ListForPanorama.json', ArrayOf(chlk.models.apps.ApplicationForPanoramaViewData), {})
            },

            ria.async.Future, function getExternalAttachApps() {
                return this.get('Application/ExternalAttachApps.json', ArrayOf(chlk.models.apps.Application), {
                        start: 0,
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
                return this.uploadFiles('Application/UploadPicture', [file], chlk.models.id.PictureId, {
                    width: height_,
                    height: width_
                });
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
                        return this.getAccessToken(personId, attachment.getUrl())
                            .then(function(data){
                                attachment.setToken(data.getToken());
                                return attachment;
                            });
                    }, this);
            },

            [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function attachApp(appAnnouncementId, announcementType_) {
              return this
                  .post('Application/Attach.json', chlk.models.announcement.FeedAnnouncementViewData, {
                      announcementApplicationId: appAnnouncementId.valueOf(),
                      announcementType: announcementType_ && announcementType_.valueOf()
                  });
            },

            [[chlk.models.id.SchoolPersonId, String, chlk.models.id.AppId]],
            ria.async.Future, function getAccessToken(personId, appUrl_, appId_){
                var forEdit = false;
                return this.get('Application/GetAccessToken.json', chlk.models.apps.ApplicationAuthorization, {
                    applicationUrl: appUrl_,
                    applicationId: appId_ ? appId_.valueOf() : undefined
                }).transform(function (applicationAuthorization) {
                    var app = applicationAuthorization.getApplication();
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
                chlk.models.id.SchoolPersonId,
                chlk.models.apps.AppAccess,
                ArrayOf(chlk.models.id.AppCategoryId),
                ArrayOf(chlk.models.id.PictureId),
                ArrayOf(chlk.models.id.GradeLevelId),
                ArrayOf(chlk.models.apps.AppPlatformTypeEnum),
                Boolean,
                ArrayOf(chlk.models.id.ABStandardId)
            ]],
            ria.async.Future, function updateApp(
                appId, shortAppInfo, permissionIds, devId, appAccess, categories, pictures_,
                gradeLevels, platforms, forSubmit, standards){
                return this.post('Application/Update.json', chlk.models.apps.Application,  {
                    applicationId: appId.valueOf(),
                    shortApplicationInfo: shortAppInfo.getPostData(),
                    permissions: this.arrayToIds(permissionIds),
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
            },

            [[
                String,
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                ArrayOf(chlk.models.standard.Standard),
                String, String, Number, Number
            ]],
            ria.async.Future, function getApplicationContents(appUrl, announcementId, announcementType, standards, encodedKey, accessToken, start_, count_){
                var params = chlk.models.standard.Standard.BUILD_URL_PARAMS_FROM_STANDARDS(standards);
                params.apiRoot = _GLOBAL.location.origin;
                params.mode = 'content-query'; // added this mode to settings
                params.announcementId = announcementId.valueOf();
                params.announcementType = announcementType.valueOf();
                params.start = start_;
                params.count = count_ || 5;
                params.token = accessToken;
                //disable Cache
                params._= Math.random().toString(36).substr(2) + (new Date).getTime().toString(36);

                var signature = this.generateTokenForApiCall_(params, encodedKey);
                return this.makeGetPaginatedListApiCall(appUrl, chlk.models.apps.ApplicationContent, signature, params);
            },

            [[Object, String]],
            String, function generateTokenForApiCall_(params, encodedKey){

                var nameValueArray = [];
                for(var key in params){
                    if(params.hasOwnProperty(key) && params[key] != null && params[key] != undefined && params[key] !== '')
                        nameValueArray.push({ name : key, value : params[key]})
                }
                nameValueArray = nameValueArray.sort(function(a, b){
                    return a.name < b.name ? -1 : (a.name > b.name ? 1 : 0) ;
                });
                var msg = nameValueArray.map(function(_){return _.value;}).join('|');
                msg += '|' + encodedKey;
                //msg += 'test not valid token test';
                return CryptoJS.SHA256(msg).toString();
            },


            [[Number, Number]],
            ria.async.Future, function getAppsForAttach(start_, count_){
                return this.getPaginatedList('Application/ListForAttach.json', chlk.models.apps.Application,{
                    start: start_ || 0,
                    count: count_ || 7
                });
            },

            [[String, String, Number, Boolean]],
            ria.async.Future, function getSuggestedApps(academicBenchmarkIds, start_, count_, myAppsOnly_){
                return this.get('Application/SuggestedApps.json', ArrayOf(chlk.models.apps.Application),{
                    abIds : academicBenchmarkIds,
                    start: start_ | 0,
                    count: count_ || 9999,
                    myAppsOnly: myAppsOnly_
                });
            },

            [[Number, Number]],
            ria.async.Future, function getMyApps(start_, count_) {
                return this.getPaginatedList('Application/MyApps.json', chlk.models.apps.Application, {
                    start: start_ | 0,
                    count: count_ || 10000,
                });
            },

            [[chlk.models.id.AppId, ArrayOf(chlk.models.id.SchoolId)]],
            function submitApplicationBan(applicationId, schoolIds){
                return this.post('Application/SubmitApplicationBan.json', Boolean, {
                    applicationId: applicationId.valueOf(),
                    schoolIds: this.arrayToCsv(schoolIds)
                });
            },

            [[chlk.models.id.AppId]],
            function getApplicationBannedSchools(applicationId){
                return this.get('Application/ApplicationBannedSchools', ArrayOf(chlk.models.apps.ApplicationSchoolBan), {
                    applicationId: applicationId.valueOf()
                });
            }
        ])
});
