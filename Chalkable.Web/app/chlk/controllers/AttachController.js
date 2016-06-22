REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.services.ApplicationService');

REQUIRE('chlk.activities.announcement.AttachFilesDialog');

NAMESPACE('chlk.controllers', function () {

    CLASS(
        'AttachController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.AttachmentService, 'attachmentService',

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
            function startWidgetAction(requestId) {

                var options = chlk.models.common.AttachOptionsViewData.$create(false, false, null, false, "");

                this.getContext().getSession().set('AttachWidget_RequestId', requestId);
                this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);

                var data = new chlk.models.common.BaseAttachViewData(options);

                var result = ria.async.Future.$fromData(data);

                return this.ShadeOrUpdateView(chlk.activities.attach.AttachFileDialog, result);
            }
        ]);
});
