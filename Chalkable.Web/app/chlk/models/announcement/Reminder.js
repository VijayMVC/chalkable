REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.Reminder*/
    CLASS(
        'Reminder', [
            Number, 'before',

            chlk.models.id.ReminderId, 'id',

            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'owner',

            [ria.serialize.SerializeProperty('reminddate')],
            chlk.models.common.ChlkDate, 'remindDate',

            Boolean, 'duplicate'
        ]);
});
