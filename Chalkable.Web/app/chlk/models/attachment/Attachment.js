REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementAttachmentId');

NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.attachment.Attachment*/
    CLASS(
        UNSAFE, FINAL, 'Attachment', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.AnnouncementAttachmentId);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.teachersAttachment = SJX.fromValue(raw.isteacherattachment, Boolean);
                this.name = SJX.fromValue(raw.name, String);
                this.order = SJX.fromValue(raw.order, Number);
                this.thumbnailUrl = SJX.fromValue(raw.thumbnailurl, String);
                this.bigUrl = SJX.fromValue(raw.bigurl, String);
                this.type = SJX.fromValue(raw.type, Number);
                this.openOnStart = SJX.fromValue(raw.openonstart, Boolean);
                this.url = SJX.fromValue(raw.url, String);
            },

            chlk.models.id.AnnouncementAttachmentId, 'id',
            Boolean, 'owner',
            Boolean, 'openOnStart',
            Boolean, 'teachersAttachment',
            String, 'name',
            Number, 'order',
            String, 'thumbnailUrl',
            String, 'bigUrl',
            Number, 'type',
            String, 'url'
        ]);
});
