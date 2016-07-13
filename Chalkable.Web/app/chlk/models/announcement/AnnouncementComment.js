REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementCommentId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AnnouncementComment*/
    CLASS(
        UNSAFE, FINAL, 'AnnouncementComment', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementCommentId);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.parentCommentId = SJX.fromValue(raw.parentcommentid, chlk.models.id.AnnouncementCommentId);
                if(raw.attachments)
                    this.attachments = SJX.fromArrayOfDeserializables(raw.attachments, chlk.models.attachment.Attachment);
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.postedDate = SJX.fromDeserializable(raw.posteddate, chlk.models.common.ChlkDate);
                this.text = SJX.fromValue(raw.text, String);
                this.attachmentIds = SJX.fromValue(raw.attachmentIds, String);
                this.hidden = SJX.fromValue(raw.hidden, Boolean);
                if(raw.subcomments)
                    this.subComments = SJX.fromArrayOfDeserializables(raw.subcomments, chlk.models.announcement.AnnouncementComment);
            },

            chlk.models.id.AnnouncementCommentId, 'id',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.AnnouncementCommentId, 'parentCommentId',
            ArrayOf(chlk.models.attachment.Attachment), 'attachments',
            chlk.models.people.User, 'owner',
            chlk.models.common.ChlkDate, 'postedDate',
            String, 'text',
            Boolean, 'hidden',
            String, 'attachmentIds',
            chlk.models.id.AttachmentId, 'attachmentId',

            ArrayOf(SELF), 'subComments',

            [[ArrayOf(chlk.models.attachment.Attachment), chlk.models.id.AnnouncementCommentId]],
            function $(attachments_, id_){
                BASE();
                attachments_ && this.setAttachments(attachments_);
                id_ && this.setId(id_);
            }
        ]);
});
