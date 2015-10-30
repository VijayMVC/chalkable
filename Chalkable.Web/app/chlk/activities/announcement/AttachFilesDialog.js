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

            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, 'delete-attachment')],
            VOID, function attachmentDelete(tpl, model, msg_) {
                this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']').remove();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementAttachmentTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function attachmentUploaded(tpl, model, msg_) {
                var node = this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']');
                ria.dom.Dom(tpl.render()).insertAfter(node);
                node.remove();
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