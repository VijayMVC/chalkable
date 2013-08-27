REQUIRE('chlk.models.announcement.AnnouncementPeriod');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.WeekItem*/
    CLASS(
        'WeekItem', [
            chlk.models.common.ChlkDate, 'date',

            Number, 'day',

            Boolean, 'sunday',

            String, 'todayClassName',

            [ria.serialize.SerializeProperty('announcementperiods')],
            ArrayOf(chlk.models.announcement.AnnouncementPeriod), 'announcementPeriods',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ]);
});
