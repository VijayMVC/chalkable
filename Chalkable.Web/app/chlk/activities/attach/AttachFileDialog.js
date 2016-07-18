REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.attach.FileAttachTpl');
REQUIRE('chlk.templates.attach.AttachmentTpl');

NAMESPACE('chlk.activities.attach', function () {

    /** @class chlk.activities.attach.AttachFileDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.attach.FileAttachTpl)],
        'AttachFileDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.PartialUpdateRule(chlk.templates.attach.AttachmentTpl, 'attachment-progress')],
            VOID, function attachmentProgress(tpl, model, msg_) {
                var node = this.dom.find('.attachment-file[data-index=' + model.getFileIndex() + ']');
                if(!node.exists()) {
                    this.dom.find('.files-container').appendChild(tpl.render());

                } else if (model.getId()) {
                    ria.dom.Dom(tpl.render()).insertAfter(node);
                    node.remove();

                } else {
                    var percents = Math.round(model.getLoaded() * 100 / model.getTotal());
                    node.find('.progress').setCss('width', percents + '%');
                }

                if (this.dom.find('#is-for-attribute').getValue()) {
                    var btn = this.dom.find('#add-file-attachment');
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                    btn.parent('.for-attribute').addClass('disabled-upload');
                    btn.parent('.upload-button').removeClass('enabled');
                }

                this.updateFileCounter_();
            },

            VOID, function updateFileCounter_() {
                var count = this.dom.find('.attachment-file').count();

                this.dom.find('.files-count').setText(count.toString());
                this.dom.find('#attach-files-btn').toggleAttr('disabled', count == 0);

                var isSingle = this.dom.find('#is-for-attribute').getValue();

                var btn = this.dom.find('#add-file-attachment');
                btn.toggleAttr('disabled', isSingle && count > 0);
                btn.setProp('disabled', isSingle && count > 0);
                btn.parent('.for-attribute').toggleClass('disabled-upload', isSingle && count > 0);
                btn.parent('.upload-button').toggleClass('enabled', !(isSingle && count > 0));
                btn.setValue('');
            },

            [ria.mvc.DomEventBind('click', '.delete-attachment')],
            VOID, function deleteAttachment(node, event) {
                node.parent().removeSelf();
                this.updateFileCounter_();
            }
        ]);
});
