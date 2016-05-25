REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.services.ApplicationService');

REQUIRE('chlk.activities.announcement.AttachFilesDialog');

NAMESPACE('chlk.controllers', function () {

    CLASS(
        'AttachController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.AttachmentService, 'attachmentService',

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'applicationService',

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN,
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.STUDENT,
            ])],
            [[
                String,
                Boolean,
                Boolean,
                chlk.models.id.AppId,
                Boolean,
                String
            ]],
            function startWidgetAction(requestId
                , fileCabinetEnabled
                , standardAttachEnabled
                , assessmentAppId
                , ableAttachApps
                , appUrlAppend_) {

                var result = this.applicationService.getExternalAttachApps()
                    .then(function(externalAttachApps){
                        //_DEBUG && options.setAssessmentAppId(chlk.models.id.AppId('56c14655-2897-4073-bb48-32dfd61264b5'));

                        var options = chlk.models.common.AttachOptionsViewData.$create(
                            fileCabinetEnabled,
                            standardAttachEnabled,
                            assessmentAppId,
                            ableAttachApps,
                            appUrlAppend_
                        );

                        options.setExternalAttachApps(externalAttachApps);

                        this.getContext().getSession().set('AttachWidget_RequestId', requestId);
                        this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);

                        return new chlk.models.common.BaseAttachViewData(options);
                    }, this);

                return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
            }
        ]);
});
