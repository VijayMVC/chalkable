REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementCommentService');
REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.services.AnnouncementAttachmentService');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementCommentController*/
    CLASS(
        'AnnouncementCommentController', EXTENDS(chlk.controllers.BaseController), [


        [ria.mvc.Inject],
        chlk.services.AnnouncementCommentService, 'announcementCommentService',

        [ria.mvc.Inject],
        chlk.services.AttachmentService, 'attachmentService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementAttachmentService, 'announcementAttachmentService',

        function prepareCommentAttachment_(attachment, width_, height_){
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachment.setThumbnailUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, width_ || 170, height_ || 110));
                attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, null, null));
            }
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.OTHER){
                attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), true, null, null));
            }
        },

        function prepareCommentsAttachments_(comments){
            var that = this;
            comments.forEach(function(comment){
                if(comment.getAttachment())
                    that.prepareCommentAttachment_(comment.getAttachment());
                if(comment.getSubComments())
                    that.prepareCommentsAttachments_(comment.getSubComments())
            });
        },

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.announcement.AnnouncementComment]],
        function postAction(model){
            var res = this.announcementCommentService
                .postComment(model.getAnnouncementId(), model.getText(), model.getAttachmentId())
                .then(function(announcement){
                    this.prepareCommentsAttachments_(announcement.getAnnouncementComments());
                    return announcement;
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'discussion');
        },

        [[chlk.models.id.AttachmentId, String, chlk.models.attachment.AttachmentTypeEnum]],
        function viewAttachmentAction(attachmentId, url, type){
            var attachmentUrl, res;
            var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                "download-attachment",
                "Download Attachment",
                this.attachmentService.getDownloadUri(attachmentId, true, null, null)
            );

            if(type == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachmentUrl = url;
                var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(
                    attachmentUrl,
                    [downloadAttachmentButton],
                    type
                );
                res = new ria.async.DeferredData(attachmentViewData);
            }else{
                var buttons = [downloadAttachmentButton];
                res = this.announcementAttachmentService
                    .startViewSession(attachmentId)
                    .then(function(session){
                        return new chlk.models.common.attachments.BaseAttachmentViewData(
                            this.attachmentService.getViewSessionUrl(session),
                            buttons,
                            type
                        );
                    }, this);
            }
            return this.ShadeView(chlk.activities.common.attachments.AttachmentDialog, res);
        },

        function uploadCommentAttachmentAction(files) {
            var res = this.attachmentService
                .uploadAttachment([files[0]])
                .then(function(attachment){
                    this.prepareCommentAttachment_(attachment);
                    return new chlk.models.announcement.AnnouncementComment(attachment);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'file-for-comment');
        }

    ])
});
