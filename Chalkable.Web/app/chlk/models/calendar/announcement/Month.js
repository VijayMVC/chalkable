REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Month*/
    CLASS(
        'Month', [
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

            chlk.models.classes.ClassesForTopBar, 'topData',

            Number, 'selectedTypeId',

            String, 'currentTitle',

            chlk.models.common.ChlkDate, 'nextDate',

            chlk.models.common.ChlkDate, 'prevDate',

            chlk.models.common.ChlkDate, 'currentDate'
        ]);
});
