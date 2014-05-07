REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.BaseCalendarMonthItem*/
    CLASS(
        'BaseCalendarMonthItem', [
            Number, 'day',
            [ria.serialize.SerializeProperty('iscurrentmonth')],
            Boolean, 'currentMonth',

            [ria.serialize.SerializeProperty('issunday')],
            Boolean, 'sunday',

            chlk.models.common.ChlkDate, 'date'

    ]);
});
