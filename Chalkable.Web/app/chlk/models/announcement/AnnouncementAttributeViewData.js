REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeAttachmentId');
REQUIRE('chlk.models.id.AnnouncementAttributeTypeId');
REQUIRE('chlk.models.id.SisAssignedAttributeId');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');

NAMESPACE('chlk.models.announcement', function(){


    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.AnnouncementAttributeAttachmentViewData*/

    CLASS(UNSAFE, FINAL, 'AnnouncementAttributeAttachmentViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.AnnouncementAssignedAttributeAttachmentId, 'id',
        String, 'name',
        String, 'url',
        String, 'thumbnailUrl',
        chlk.models.attachment.AttachmentTypeEnum, 'type',
        String, 'uuid',
        String, 'mimeType',
        Boolean, 'stiAttachment',

        //
        Boolean, 'readOnly',
        chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',
        chlk.models.id.AnnouncementId, 'announcementId',
        chlk.models.id.AnnouncementAssignedAttributeId, 'attributeId',

        

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAssignedAttributeAttachmentId);
            this.name = SJX.fromValue(raw.name, String);
            this.url = SJX.fromValue(raw.url, String);
            this.uuid = SJX.fromValue(raw.uuid, String);
            this.thumbnailUrl = SJX.fromValue(raw.thumbnailurl, String);
            this.type = SJX.fromValue(raw.type, chlk.models.attachment.AttachmentTypeEnum);
            this.stiAttachment = SJX.fromValue(raw.stiattachment, Boolean);
            this.mimeType = SJX.fromValue(raw.mimetype, String);
        },

        Object, function getPostData() {
            return {
                name: this.getName(),
                id: this.getId() && this.getId().valueOf(),
                stiattachment: this.isStiAttachment(),
                uuid: this.getUuid(),
                mimetype: this.getMimeType(),
                //ignored on server
                url: this.getUrl(),
                thumbnailurl: this.getThumbnailUrl(),
                type: this.getType()
            }
        },

        [[Object]],
        function $fromObject(obj){
            BASE();
            this.deserialize(obj);
        }
    ]);

    /**@class chlk.models.announcement.AnnouncementAttributeViewData*/

    CLASS(UNSAFE, FINAL, 'AnnouncementAttributeViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        chlk.models.id.AnnouncementAssignedAttributeId, 'id',

        String, 'name',

        String, 'text',

        String, 'uuid',

        Boolean, 'visibleForStudents',

        chlk.models.id.AnnouncementAttributeTypeId, 'attributeTypeId',

        chlk.models.id.AnnouncementId, 'announcementRef',

        chlk.models.announcement.AnnouncementAttributeAttachmentViewData, 'attributeAttachment',

        ////

        Boolean, 'deleted',

        Boolean, 'readOnly',

        Number, 'index',

        chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

        chlk.models.id.AnnouncementId, 'announcementId',

        chlk.models.id.SisAssignedAttributeId, 'sisActivityAssignedAttributeId',

        ArrayOf(chlk.models.announcement.AnnouncementAttributeType), 'attributeTypes',

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAssignedAttributeId);
            this.name = SJX.fromValue(raw.name, String);
            this.text = SJX.fromValue(raw.text, String);
            this.uuid = SJX.fromValue(raw.uuid, String);
            this.visibleForStudents = SJX.fromValue(raw.visibleforstudents, Boolean);
            this.attributeTypeId = SJX.fromValue(raw.attributetypeid, chlk.models.id.AnnouncementAttributeTypeId);
            this.attributeAttachment = SJX.fromDeserializable(raw.attributeattachment, chlk.models.announcement.AnnouncementAttributeAttachmentViewData);
            this.announcementRef = SJX.fromValue(raw.announcementref, chlk.models.id.AnnouncementId);
            this.sisActivityAssignedAttributeId = SJX.fromValue(raw.sisactivityassignedattributeid, chlk.models.id.SisAssignedAttributeId);
            this.announcementType = SJX.fromValue(raw.announcementtype, chlk.models.announcement.AnnouncementTypeEnum);
        },

        Object, function getPostData(){
            return {
                name: this.getName() || '',
                text: this.getText() || '',
                uuid: this.getUuid() || '',
                visibleforstudents: this.isVisibleForStudents(),
                attributetypeid: this.getAttributeTypeId().valueOf(),
                id: this.getId().valueOf(),
                announcementid: this.getAnnouncementRef().valueOf(),
                attachmentid: this.getAttributeAttachment().getId() ? this.getAttributeAttachment().getId().valueOf() : null,
                attributeattachment: this.getAttributeAttachment().getId() ? this.getAttributeAttachment().getPostData() : null
            }
        },

        [[Object]],
        function $fromObject(obj) {
            BASE();
            this.deserialize(obj);

        },


        [[chlk.models.id.AnnouncementAssignedAttributeId, Boolean]],
        function $fromId(id, deleted){
            BASE();
            this.setId(id);
            this.setDeleted(deleted);
        }
    ]);
});