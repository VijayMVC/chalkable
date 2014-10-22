REQUIRE('chlk.models.id.AnnouncementAttachmentId');
NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.Attachment*/
    CLASS(
        'Attachment', [
            chlk.models.id.AnnouncementAttachmentId, 'id',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'owner',

            Boolean, 'openOnStart',

            [ria.serialize.SerializeProperty('isteacherattachment')],
            Boolean, 'teachersAttachment',

            String, 'name',

            Number, 'order',

            [ria.serialize.SerializeProperty('thumbnailurl')],
            String, 'thumbnailUrl',

            String, 'bigUrl',

            Number, 'type',

            String, 'url'
        ]);
});
