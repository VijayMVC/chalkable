REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.ListForWeekCalendarItem*/
    CLASS(
        'ListForWeekCalendarItem', [

            Number, 'day',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('dayofweek')],
            Number, 'dayOfWeek',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ]);
});
