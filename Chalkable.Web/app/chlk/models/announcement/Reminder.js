REQUIRE('chlk.models.id.ReminderId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.Reminder*/
    CLASS(
        'Reminder', [
            Number, 'before',

            chlk.models.id.ReminderId, 'id',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'isOwner',

            [ria.serialize.SerializeProperty('reminddate')],
            chlk.models.common.ChlkDate, 'remindDate'
        ]);
});
