REQUIRE('chlk.models.calendar.announcement.CalendarDayItem');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.DayItem*/
    CLASS(
        'DayItem', [
            chlk.models.common.ChlkDate, 'date',

            Number, 'day',

            [ria.serialize.SerializeProperty('calendardayitems')],
            ArrayOf(chlk.models.calendar.announcement.CalendarDayItem), 'calendarDayItems'
        ]);
});
