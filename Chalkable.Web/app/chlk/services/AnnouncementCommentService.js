REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.announcement.AnnouncementComment');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementCommentService */
    CLASS(
        'AnnouncementCommentService', EXTENDS(chlk.services.BaseService), [


            [[chlk.models.id.AnnouncementId, String, chlk.models.id.AttachmentId]],
            ria.async.Future, function postComment(announcementId, text, attachmentId_){
                return  this.post('AnnouncementComment/PostComment', chlk.models.announcement.AnnouncementComment,{
                    announcementId: announcementId.valueOf(),
                    text: text,
                    attachmentId: attachmentId_ && attachmentId_.valueOf()
                })
                .then(function(comment){
                    var res = this.getAnnouncement_(comment.getAnnouncementId());
                    res.getAnnouncementComments().push(comment);
                    return res;
                }, this);
            },


            [[chlk.models.id.AnnouncementCommentId, String, chlk.models.id.AttachmentId]],
            ria.async.Future, function edit(announcementCommentId, text, attachmentId){
                return this.post('AnnouncementComment/Edit', chlk.models.announcement.AnnouncementComment, {
                    announcementCommentId: announcementCommentId.valueOf(),
                    text: text,
                    attachmentId: attachmentId && attachmentId.valueOf()
                })
                .then(function(comment){
                    return this.editCacheAnnouncementComments_(comment);
                }, this);
            },


            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId, Boolean]],
            ria.async.Future, function setHidden(announcementId, announcementCommentId, hidden){
                return this.post('AnnouncementComment/SetHidden', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf(),
                    hidden: hidden,
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                });
            },


            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId]],
            ria.async.Future, function deleteComment(announcementId, announcementCommentId){
                return this.post('AnnouncementComment/Delete', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf(),
                    hidden: hidden,
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                });
            },

            //TODO rewrite this ... think about recursive
            [[chlk.models.announcement.AnnouncementComment]],
            Object, function editCacheAnnouncementComments_(comment){
                var result =this.getAnnouncement_(comment.getAnnouncementId());
                for (var i = 0; i < result.getAnnouncementComments().length; i++)
                    if (result.getAnnouncementComments()[i].getId() == comment.getId())
                    {
                        result.getAnnouncementComments()[i] = comment;
                        break;
                    }
                return result;
            },

            [[chlk.models.id.AnnouncementId]],
            chlk.models.announcement.AnnouncementView, function getAnnouncement_(id){
                return this.getContext().getService(chlk.services.AnnouncementService).getAnnouncementSync(id);
            }
        ]);
});
