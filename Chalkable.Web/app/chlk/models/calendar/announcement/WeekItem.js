REQUIRE('chlk.models.announcement.AnnouncementPeriod');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.Popup');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.WeekItem*/
    CLASS(
        'WeekItem', EXTENDS(chlk.models.Popup), [
            chlk.models.common.ChlkDate, 'date',

            Number, 'day',

            [ria.serialize.SerializeProperty('dayofweek')],
            Number, 'dayOfWeek',

            Boolean, 'sunday',

            String, 'todayClassName',

            chlk.models.id.ClassId, 'selectedClassId',

            [ria.serialize.SerializeProperty('announcementperiods')],
            ArrayOf(chlk.models.announcement.AnnouncementPeriod), 'announcementPeriods',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ]);
});
