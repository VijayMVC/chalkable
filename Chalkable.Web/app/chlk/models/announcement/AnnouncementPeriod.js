REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementPeriod*/
    CLASS(
        'AnnouncementPeriod', [
            chlk.models.period.Period, 'period',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            Number, 'index',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ]);
});
