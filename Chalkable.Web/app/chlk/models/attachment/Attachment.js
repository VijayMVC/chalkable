REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.attachment.AttachmentTypeEnum */
    ENUM('AttachmentTypeEnum', {
        DOCUMENT: 0,
        PICTURE: 1,
        OTHER: 2
    });

    /** @class chlk.models.attachment.AnnouncementAttachment*/
    CLASS(
        UNSAFE, FINAL, 'AnnouncementAttachment', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAttachmentId);
                this.attachmentId = SJX.fromValue(raw.attachment.id, chlk.models.id.AttachmentId);
                this.owner = SJX.fromValue(raw.attachment.isowner, Boolean);
                this.teachersAttachment = SJX.fromValue(raw.attachment.isteacherattachment, Boolean);
                this.name = SJX.fromValue(raw.attachment.name, String);
                this.order = SJX.fromValue(raw.order, Number);
                this.thumbnailUrl = SJX.fromValue(raw.attachment.thumbnailurl, String);
                this.bigUrl = SJX.fromValue(raw.attachment.bigurl, String);
                this.type = SJX.fromValue(raw.attachment.type, chlk.models.attachment.AttachmentTypeEnum);
                this.openOnStart = SJX.fromValue(raw.attachment.openonstart, Boolean);
                this.url = SJX.fromValue(raw.attachment.url, String);
            },

            [[Number, Number, Number, String]],
            function $(fileIndex_, total_, loaded_, name_){
                BASE();
                if(fileIndex_ || fileIndex_ === 0)
                    this.setFileIndex(fileIndex_);
                if(total_)
                    this.setTotal(total_);
                if(loaded_)
                    this.setLoaded(loaded_);
                if(name_)
                    this.setName(name_);
            },

            chlk.models.id.AnnouncementAttachmentId, 'id',
            chlk.models.id.AttachmentId, 'attachmentId',

            Boolean, 'owner',
            Boolean, 'openOnStart',
            Boolean, 'teachersAttachment',
            String, 'name',
            Number, 'order',
            String, 'thumbnailUrl',
            String, 'bigUrl',
            chlk.models.attachment.AttachmentTypeEnum, 'type',
            String, 'url',
            Number, 'fileIndex',
            Number, 'total',
            Number, 'loaded',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',
            Boolean, 'attributeAttachment'
        ]);

    CLASS(
        /** @class chlk.models.attachment.Attachment*/
        UNSAFE, FINAL, 'Attachment', IMPLEMENTS(ria.serialize.IDeserializable),[

            chlk.models.id.AttachmentId, 'id',
            chlk.models.common.ChlkDate, 'uploadedDate',
            chlk.models.common.ChlkDate, 'lastAttachedDate',
            String, 'name',
            chlk.models.id.SchoolPersonId, 'personId',
            String, 'thumbnailUrl',
            String, 'url',
            String, 'publicUrl',
            String, 'pictureId',
            chlk.models.attachment.AttachmentTypeEnum, 'type',
            Boolean, 'teachersAttachment',
            Boolean, 'owner',
            Boolean, 'stiAttachment',

            Number, 'fileIndex',
            Number, 'total',
            Number, 'loaded',

            VOID, function deserialize(raw){

                this.id = SJX.fromValue(raw.id, chlk.models.id.AttachmentId);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.teachersAttachment = SJX.fromValue(raw.isteacherattachment, Boolean);
                this.name = SJX.fromValue(raw.name, String);
                this.thumbnailUrl = SJX.fromValue(raw.thumbnailurl, String);
                this.type = SJX.fromValue(raw.type, chlk.models.attachment.AttachmentTypeEnum);
                this.url = SJX.fromValue(raw.url, String);
                this.publicUrl = SJX.fromValue(raw.publicurl, String);
                this.uploadedDate = SJX.fromDeserializable(raw.uploaded, chlk.models.common.ChlkDate);
                this.lastAttachedDate = SJX.fromDeserializable(raw.lastattached, chlk.models.common.ChlkDate);
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.stiAttachment = SJX.fromValue(raw.stiattachment, Boolean);
            },

            Object, function serialize(){
                return {
                    id: this.id.valueOf(),
                    name: this.name,
                    thumbnailUrl: this.thumbnailUrl,
                    type: this.type.valueOf(),
                    url: this.url,
                    publicUrl: this.publicUrl
                }
            }
    ]);

    /** @class  chlk.models.attachment.SortAttachmentType*/
    ENUM('SortAttachmentType',{
        RECENTLY_SENT: 1,
        NEWEST_UPLOADED: 2,
        OLDEST_UPLOADED: 3
    });
});
