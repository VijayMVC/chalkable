REQUIRE('chlk.models.calendar.announcement.WeekItem');


NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Week*/
    CLASS(
        'Week', [
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items',

            chlk.models.class.ClassesForTopBar, 'topData',

            Number, 'selectedTypeId',

            String, 'currentTitle',

            chlk.models.common.ChlkDate, 'nextDate',

            chlk.models.common.ChlkDate, 'prevDate',

            chlk.models.common.ChlkDate, 'currentDate'
        ]);
});
