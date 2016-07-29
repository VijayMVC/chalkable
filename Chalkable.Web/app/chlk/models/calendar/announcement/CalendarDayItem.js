REQUIRE('chlk.models.period.Period');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.CalendarDayItem*/
    CLASS(
        'CalendarDayItem', EXTENDS(chlk.models.Popup), [
            chlk.models.period.Period, 'period',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
