REQUIRE('chlk.models.id.AnnouncementAttachmentId');
NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.Attachment*/
    CLASS(
        'Attachment', [
            chlk.models.id.AnnouncementAttachmentId, 'id',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'isOwner',

            String, 'name',

            Number, 'order',

            [ria.serialize.SerializeProperty('thumbnailurl')],
            String, 'thumbnailUrl',

            Number, 'type',

            String, 'url'
        ]);
});
