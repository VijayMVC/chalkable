REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.class.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Month*/
    CLASS(
        'Month', [
            ArrayOf(chlk.models.calendar.announcement.Day), 'days',

            chlk.models.class.ClassesForTopBar, 'topData',

            Number, 'selectedTypeId',

            String, 'currentMonth',

            chlk.models.common.ChlkDate, 'nextMonthDate',

            chlk.models.common.ChlkDate, 'prevMonthDate',

            chlk.models.common.ChlkDate, 'currentDate'
        ]);
});
