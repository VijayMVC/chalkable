REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.FileAttachTpl');
REQUIRE('chlk.templates.announcement.AnnouncementAttachmentTpl');

NAMESPACE('chlk.activities.announcement', function () {

    /** @class chlk.activities.announcement.AttachFilesDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.FileAttachTpl)],
        'AttachFilesDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, 'attachment-progress')],
            VOID, function attachmentProgress(tpl, model, msg_) {
                var node = this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']');
                if(node.exists()){
                    var percents = model.getLoaded() * 100 / model.getTotal();
                    node.find('.progress').setCss('width', percents + '%');
                }else{
                    this.dom.find('.files-container').appendChild(tpl.render());
                }

                if(this.dom.find('#is-for-attribute').getValue()){
                    var btn = this.dom.find('#add-file-attachment');
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                    btn.parent('.for-attribute').addClass('disabled-upload');
                    btn.parent('.upload-button').removeClass('enabled');
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, 'delete-attachment')],
            VOID, function attachmentDelete(tpl, model, msg_) {
                var btn = this.dom.find('#add-file-attachment');

                this.removeAttachmentFromProgressBar_(model);
                var countNode = this.dom.find('.files-count');
                countNode.setHTML((parseInt(countNode.getText(), 10) - 1).toString());
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, 'cancel-attachment-upload')],
            VOID, function cancelAttachmentUpload(tpl, model, msg_){
               this.removeAttachmentFromProgressBar_(model);
            },


            VOID, function removeAttachmentFromProgressBar_(model){
                this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']').remove();
                if(this.dom.find('#is-for-attribute').getValue()){
                    btn.removeAttr('disabled');
                    btn.setProp('disabled', false);
                    btn.parent('.for-attribute').removeClass('disabled-upload');
                    btn.parent('.upload-button').addClass('enabled');
                }
                btn.setValue('');

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function attachmentUploaded(tpl, model, msg_) {
                var node = this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']');
                ria.dom.Dom(tpl.render()).insertAfter(node);
                node.remove();
                var countNode = this.dom.find('.files-count');
                countNode.setHTML((parseInt(countNode.getText(), 10) + 1).toString());
                this.dom.find('#add-file-attachment').setValue('');
            },

            [ria.mvc.DomEventBind('click', '.delete-attachment')],
            [[ria.dom.Dom, ria.dom.Event]],
            function deleteClick(node, event){
                node.parent('.attachment-file').addClass('pending').addClass('deleting');
            },

            OVERRIDE, VOID, function onStop_() {
                if(this.dom.find('.attachment-file').count())
                    this.dom.find('.refresh-attachments').trigger('click');
                BASE();
            }
        ]);
});