NAMESPACE('chlk.models.attachment', function () {
    "use strict";

    /** @class chlk.models.attachment.AttachmentId*/
    IDENTIFIER('AttachmentId');

    /** @class chlk.models.attachment.Attachment*/
    CLASS(
        'Attachment', [
            chlk.models.attachment.AttachmentId, 'id',

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
