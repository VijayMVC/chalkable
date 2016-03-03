REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');

REQUIRE('chlk.activities.apps.ExternalAttachAppDialog');
REQUIRE('chlk.activities.apps.AppWrapperPage');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppModes');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AppPermissionId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AssessmentController */
    CLASS(
        'AssessmentController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [chlk.controllers.AssessmentEnabled()],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [[chlk.models.id.AnnouncementId, chlk.models.id.AppId, chlk.models.announcement.AnnouncementTypeEnum, String]],
            function attachAction(announcementId, appId, announcementType, appUrlAppend_) {
                var result = this.appsService
                    .addToAnnouncement(this.getCurrentPerson().getId(), appId, announcementId, announcementType)
                    .catchError(function(error_){
                        throw new chlk.lib.exception.AppErrorException(error_);
                    }, this)
                    .attach(this.validateResponse_())
                    .then(function(app){
                        var mode = chlk.models.apps.AppModes.EDIT;
                        var editUrl = this.getAbsoluteAppUrl(app.getUrl(), mode, app.getOauthCode(), app.getAnnouncementApplicationId(), null, appUrlAppend_);
                        var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS);
                        return new chlk.models.apps.ExternalAttachAppViewData(options, app
                            , editUrl, 'Attach ' + app.getName(), app.getAnnouncementApplicationId());
                    }, this);

                return this.ShadeView(chlk.activities.apps.ExternalAttachAppDialog, result);
            },



            [chlk.controllers.AssessmentEnabled()],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.DISTRICTADMIN
            ])],
            [[String,  chlk.models.id.AnnouncementApplicationId, chlk.models.id.SchoolPersonId]],
            function gradingViewAction(url, announcementAppId, studentId_){
                return this.fromItemView_(url, chlk.models.apps.AppModes.GRADINGVIEW, announcementAppId, studentId_);
            },

            [chlk.controllers.AssessmentEnabled()],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.DISTRICTADMIN,
                chlk.models.common.RoleEnum.STUDENT
            ])],
            function viewAction(url, announcementAppId){
                return this.fromItemView_(url, chlk.models.apps.AppModes.VIEW, announcementAppId);
            },

            [[String, chlk.models.apps.AppModes, chlk.models.id.AnnouncementApplicationId,  chlk.models.id.SchoolPersonId]],
            function fromItemView_(url,  mode, announcementAppId_, studentId_) {
                var result = this.appsService
                    .getOauthCode(this.getCurrentPerson().getId(), url)
                    .catchError(function(error_){
                        throw new chlk.lib.exception.AppErrorException(error_);
                    }, this)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var appData = data.getApplication();
                        var viewUrl = this.appsService.getAbsoluteAppUrl(appData.getUrl(), mode, appData.getAuthorizationCode(), null, null, announcementAppId_, studentId_);

                        var app = new chlk.models.apps.AppAttachment.$create(
                            viewUrl,
                            data.getAuthorizationCode(),
                            announcementAppId_,
                            appData
                        );
                        return new chlk.models.apps.AppWrapperViewData(app, mode);
                    }, this);

                this.userTrackingService.tookAssessment();
                return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
            },


            [chlk.controllers.AssessmentEnabled()],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN,
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.STUDENT
            ])],
            [chlk.controllers.SidebarButton('assessment')],
            [[String]],
            function myViewAction() {
                var result = this.appsService
                    .getOauthCode(this.getCurrentPerson().getId(), null, this.getAssessmentAppId())
                    .catchError(function(error_){
                        throw new chlk.lib.exception.AppErrorException(error_);
                    })
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var appData = data.getApplication();
                        var viewUrl = this.appsService.getAbsoluteAppUrl(appData.getUrl(), chlk.models.apps.AppModes.MY_VIEW, data.getAuthorizationCode());

                        return new chlk.models.apps.ExternalAttachAppViewData(null, appData, viewUrl, '');
                    }, this);
                return this.PushOrUpdateView(chlk.activities.apps.AppWrapperPage, result);
            },

            [chlk.controllers.SidebarButton('assessment')],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [[String]],
            function settingsAction() {
                var result = this.appsService
                    .getOauthCode(this.getCurrentPerson().getId(), null, this.getAssessmentAppId())
                    .catchError(function(error_){
                        throw new chlk.lib.exception.AppErrorException(error_);
                    })
                    .attach(this.validateResponse_())
                    .then(function(data){

                        var appData = data.getApplication();
                        var viewUrl = this.appsService.getAbsoluteAppUrl(appData.getUrl(), chlk.models.apps.AppModes.SYSADMIN_VIEW, data.getAuthorizationCode());
                        return new chlk.models.apps.ExternalAttachAppViewData(null, appData, viewUrl, '');
                    }, this);
                return this.PushOrUpdateView(chlk.activities.apps.AppWrapperPage, result);
            }
        ])
});
