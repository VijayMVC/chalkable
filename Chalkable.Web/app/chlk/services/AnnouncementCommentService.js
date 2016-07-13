REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.announcement.AnnouncementComment');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementCommentService */
    CLASS(
        'AnnouncementCommentService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.AnnouncementId, String, String]],
            ria.async.Future, function postComment(announcementId, text, attachmentIds_){
                return  this.post('AnnouncementComment/PostComment', chlk.models.announcement.AnnouncementComment,{
                    announcementId: announcementId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                })
                .then(function(comment){
                    var res = this.getAnnouncement_(comment.getAnnouncementId());
                    res.getAnnouncementComments().push(comment);
                    return res;
                }, this);
            },

            [[chlk.models.id.AnnouncementCommentId, String, String]],
            ria.async.Future, function reply(toAnnouncementCommentId, text, attachmentIds_){
                return this.post('AnnouncementComment/Reply', chlk.models.announcement.AnnouncementComment, {
                    toCommentId: toAnnouncementCommentId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                }).then(function(comment){
                    var result =this.getAnnouncement_(comment.getAnnouncementId());
                    var parentComment = this.findComment_(comment.getParentCommentId(), result.getAnnouncementComments());
                    var comments = parentComment.getSubComments() || [];
                    comments.push(comment);
                    parentComment.setSubComments(comments);
                    return result
                }, this);
            },

            [[chlk.models.id.AnnouncementCommentId, String, String]],
            ria.async.Future, function edit(announcementCommentId, text, attachmentIds_){
                return this.post('AnnouncementComment/Edit', chlk.models.announcement.AnnouncementComment, {
                    announcementCommentId: announcementCommentId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                })
                .then(function(comment){
                    return this.editCacheAnnouncementComments_(comment);
                }, this);
            },


            [[chlk.models.id.AnnouncementCommentId, Boolean]],
            ria.async.Future, function setHidden(announcementCommentId, hidden){
                return this.post('AnnouncementComment/SetHidden', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf(),
                    hidden: hidden
                })
                .then(function(comments){
                    var res = this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, null);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },


            [[chlk.models.id.AnnouncementCommentId]],
            ria.async.Future, function deleteComment(announcementCommentId){
                return this.post('AnnouncementComment/Delete', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf()
                })
                .then(function(comments){
                    var res = this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, null);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },

            //TODO rewrite this ... think about recursive
            [[chlk.models.announcement.AnnouncementComment]],
            Object, function editCacheAnnouncementComments_(comment){
                var result =this.getAnnouncement_(comment.getAnnouncementId());
                var selectedComment = this.findComment_(comment.getId(), result.getAnnouncementComments());
                selectedComment.setAttachment(comment.getAttachment());
                selectedComment.setText(comment.getText());
                selectedComment.setHidden(comment.isHidden());

                return result;
            },

            chlk.models.announcement.AnnouncementComment, function findComment_(id, comments){
                if(comments){
                    for(var i = 0; i < comments.length; i++){
                        if(comments[i].getId() == id)
                            return comments[i];
                        var res =this.findComment_(id, comments[i].getSubComments());
                        if(res)
                            return res;
                    }
                }
                return null;
            },

            [[chlk.models.id.AnnouncementId]],
            chlk.models.announcement.AnnouncementView, function getAnnouncement_(id){
                return this.getContext().getService(chlk.services.AnnouncementService).getAnnouncementSync(id);
            }
        ]);
});
