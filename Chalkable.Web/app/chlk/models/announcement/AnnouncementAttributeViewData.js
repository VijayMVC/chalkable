REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeAttachmentId');
REQUIRE('chlk.models.id.AnnouncementAttributeTypeId');

NAMESPACE('chlk.models.announcement', function(){


    var SJX = ria.serialize.SJX;

    /**@class chlk.models.announcement.AssignedAttributeAttachmentViewData*/

    UNSAFE, FINAL, CLASS('AnnouncementAttributeAttachmentViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.AnnouncementAssignedAttributeAttachmentId, 'id',
        String, 'name',
        String, 'url',
        String, 'thumbnailUrl',
        Number, 'type',

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAssignedAttributeAttachmentId);
            this.name = SJX.fromValue(raw.name, String);
            this.url = SJX.fromValue(raw.url, String);
            this.thumbnailUrl = SJX.fromValue(raw.thumbnailUrl, String);
            this.type = SJX.fromValue(raw.type, Number);
        }
    ]);

    /**@class chlk.models.announcement.AnnouncementAttributeViewData*/

    UNSAFE, FINAL, CLASS('AnnouncementAttributeViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        chlk.models.id.AnnouncementAssignedAttributeId, 'id',

        String, 'name',

        String, 'text',

        String, 'uuid',

        Boolean, 'visibleForStudents',

        chlk.models.id.AnnouncementAttributeTypeId, 'attributeTypeId',

        chlk.models.announcement.AnnouncementAttributeAttachmentViewData, 'attributeAttachment',


        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAssignedAttributeId);
            this.name = SJX.fromValue(raw.name, String);
            this.text = SJX.fromValue(raw.text, String);
            this.uuid = SJX.fromValue(raw.uuid, String);
            this.visibleForStudents = SJX.fromValue(raw.visibleforstudents, Boolean);
            this.attributeTypeId = SJX.fromValue(raw.attributetypeid, chlk.models.id.AnnouncementAttributeTypeId);
            this.attributeAttachment = SJX.fromDeserializable(raw.attributeattachment, chlk.models.announcement.AnnouncementAttributeAttachmentViewData);
        },


        Object, function getPostData(){
            return {
                name: this.getName() || '',
                text: this.getText() || '',
                uuid: this.getUuid() || '',
                visibleforstudents: this.isVisibleForStudents(),
                attributyTypeId: this.getAttributeTypeId().valueOf()
            }
        }
    ]);
});