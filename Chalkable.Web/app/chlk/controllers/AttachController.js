REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.services.ApplicationService');

REQUIRE('chlk.activities.attach.AttachFileDialog');

NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.AttachController */
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

                var data = new chlk.models.common.BaseAttachViewData();

                data.setRequestId(requestId);

                var result = ria.async.Future.$fromData(data);

                return this.ShadeOrUpdateView(chlk.activities.attach.AttachFileDialog, result);
            },

            [[Number, Object]],
            function uploadFileAction(fileIndex, files) {

                var all = [].slice.call(files).map(function (file, index) {
                    var model = new chlk.models.attachment.Attachment();
                    model.setFileIndex(fileIndex + index);
                    model.setTotal(file.size);
                    model.setLoaded(0);
                    model.setName(file.name);
                    this.BackgroundUpdateView(chlk.activities.attach.AttachFileDialog, model, 'attachment-progress');

                    return this.attachmentService
                        .uploadAttachment([file])
                        .handleProgress(function(event) {
                            var model = new chlk.models.attachment.Attachment();
                            model.setFileIndex(fileIndex + index);
                            model.setTotal(event.total);
                            model.setLoaded(event.loaded);
                            model.setName(file.name);
                            this.BackgroundUpdateView(chlk.activities.attach.AttachFileDialog, model, 'attachment-progress');
                        }, this)
                        .catchError(this.handleExceptions_)
                        .attach(this.validateResponse_())
                        .then(function (model) {
                            model.setFileIndex(fileIndex + index);
                            model.setTotal(file.size);
                            model.setLoaded(file.size);
                            this.BackgroundUpdateView(chlk.activities.attach.AttachFileDialog, model, 'attachment-progress');
                            return model;
                        }, this);

                }.bind(this));

                //ria.async.wait(all)
                //    .then(function(data) {
                //
                //    }, this);

                return null;
            },

            [[Object]],
            function completeAction(data) {
                console.log(data.id);

                this.BackgroundCloseView(chlk.activities.attach.AttachFileDialog);
                this.WidgetComplete(data.requestId, data.id);
            },

            function handleExceptions_(error){
                throw error;
            }
        ]);
});
