REQUIRE('chlk.models.common.ChlkDate');

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
            chlk.models.common.ChlkDate, 'date'
            //schedule section
            //announcements
            //items
        ]);
});
