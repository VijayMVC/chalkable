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
                return  this.post('AnnouncementComment/PostComment', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementId: announcementId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId, String, String]],
            ria.async.Future, function reply(announcementId, toAnnouncementCommentId, text, attachmentIds_){
                return this.post('AnnouncementComment/Reply', ArrayOf(chlk.models.announcement.AnnouncementComment), {
                    toCommentId: toAnnouncementCommentId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                }).then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId, String, String]],
            ria.async.Future, function edit(announcementId, announcementCommentId, text, attachmentIds_){
                return this.post('AnnouncementComment/Edit', ArrayOf(chlk.models.announcement.AnnouncementComment), {
                    announcementCommentId: announcementCommentId.valueOf(),
                    text: text,
                    attachmentIds: attachmentIds_
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },


            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId, Boolean]],
            ria.async.Future, function setHidden(announcementId, announcementCommentId, hidden){
                return this.post('AnnouncementComment/SetHidden', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf(),
                    hidden: hidden
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },


            [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementCommentId]],
            ria.async.Future, function deleteComment(announcementId, announcementCommentId){
                return this.post('AnnouncementComment/Delete', ArrayOf(chlk.models.announcement.AnnouncementComment),{
                    announcementCommentId: announcementCommentId.valueOf()
                })
                .then(function(comments){
                    var res = this.getAnnouncement_(announcementId);
                    res.setAnnouncementComments(comments);
                    return res;
                }, this);
            },

            //TODO rewrite this ... think about recursive
            [[chlk.models.announcement.AnnouncementComment]],
            Object, function editCacheAnnouncementComments_(comment){
                var result =this.getAnnouncement_(comment.getAnnouncementId());
                var selectedComment = this.findComment_(comment.getId(), result.getAnnouncementComments());
                selectedComment.setAttachments(comment.getAttachments());
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
            function getAnnouncement_(id){
                return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, null);
            }
        ]);
});
