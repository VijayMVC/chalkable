REQUIRE('chlk.models.calendar.announcement.DayItem');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Day*/
    CLASS(
        'Day',  [
            chlk.models.class.ClassesForTopBar, 'topData',

            Number, 'selectedTypeId',

            String, 'currentTitle',

            chlk.models.common.ChlkDate, 'nextDate',

            chlk.models.common.ChlkDate, 'prevDate',

            chlk.models.common.ChlkDate, 'currentDate',
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items'
        ]);
});
