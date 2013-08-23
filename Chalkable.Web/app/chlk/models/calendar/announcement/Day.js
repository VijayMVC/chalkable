REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.schoolYear.ScheduleSection');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Day*/
    CLASS(
        'Day', [
            Number, 'day',
            [ria.serialize.SerializeProperty('iscurrentmonth')],
            Boolean, 'currentMonth',

            [ria.serialize.SerializeProperty('issunday')],
            Boolean, 'sunday',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('schedulesection')],
            chlk.models.schoolYear.ScheduleSection, 'scheduleSection',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            ArrayOf(chlk.models.announcement.Announcement), 'items',

            String, 'todayClassName',

            String, 'role',

            Number, 'annLimit',

            String, 'className',

            Array, 'itemsArray'
        ]);
});
