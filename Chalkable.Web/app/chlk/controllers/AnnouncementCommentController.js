REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementCommentService');
REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementCommentController*/
    CLASS(
        'AnnouncementCommentController', EXTENDS(chlk.controllers.BaseController), [


        [ria.mvc.Inject],
        chlk.services.AnnouncementCommentService, 'announcementCommentService',

        [ria.mvc.Inject],
        chlk.services.AttachmentService, 'attachmentService',

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
                if(comment.getAttachments())
                    comment.getAttachments().forEach(function(attachment){
                        that.prepareCommentAttachment_(attachment);
                    });

                if(comment.getSubComments())
                    that.prepareCommentsAttachments_(comment.getSubComments())
            });
        },

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.announcement.AnnouncementComment]],
        function postAction(model){

            if(!model.getText() && !model.getAttachmentIds()){
                this.ShowMsgBox('Please enter text or add attachment', 'whoa.');
                return null;
            }

            var method, id;
            if(model.getId() && model.getId().valueOf()){
                method = 'edit';
                id = model.getId();
            }else{
                if(model.getParentCommentId() && model.getParentCommentId().valueOf()){
                    method = 'reply';
                    id = model.getParentCommentId();
                }else{
                    method = 'postComment';
                    id = model.getAnnouncementId();
                }
            }

            var res = this.announcementCommentService[method]
                (id, model.getText(), model.getAttachmentIds())
                .then(function(announcement){
                    this.prepareCommentsAttachments_(announcement.getAnnouncementComments());
                    return announcement;
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'discussion');
        },

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.id.AnnouncementCommentId]],
        function deleteCommentAction(announcementCommentId){
            var res = this.announcementCommentService
                .deleteComment(announcementCommentId)
                .then(function(announcement){
                    this.prepareCommentsAttachments_(announcement.getAnnouncementComments());
                    return announcement;
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'discussion');
        },

        [chlk.controllers.NotChangedSidebarButton],
        [[chlk.models.id.AnnouncementCommentId, Boolean]],
        function setCommentHiddenAction(announcementCommentId, hidden){
            var res = this.announcementCommentService
                .setHidden(announcementCommentId, hidden)
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
                res = this.attachmentService
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

        function uploadCommentAttachmentAction(commentId_, files_) {
            if(files_){
                var arr = [], len = files_.length, that = this;
                for(var i = 0; i < len; i++){
                    arr.push(this.attachmentService.uploadAttachment([files_[i]]));
                }
                var res = ria.async.wait(arr)
                    .then(function(attachments){
                        attachments.forEach(function(attachment){
                            that.prepareCommentAttachment_(attachment);
                        });
                        return new chlk.models.announcement.AnnouncementComment(attachments, commentId_);
                    })
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'file-for-comment');
            }

            return null;
        }

    ])
});
