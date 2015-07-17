REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.BaseCalendar*/
    CLASS(
        'BaseCalendar', [

            Number, 'selectedTypeId',

            String, 'currentTitle',

            chlk.models.common.ChlkDate, 'nextDate',

            chlk.models.common.ChlkDate, 'prevDate',

            chlk.models.common.ChlkDate, 'currentDate',

            [[chlk.models.common.ChlkDate]],
            VOID, function setCurrentDate(date){
                this.currentDate = date || new chlk.models.common.ChlkSchoolYearDate();
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            VOID, function setBaseCalendarData(date, minDate, maxDate){
                this.setCurrentDate(date);
            }
        ]);
});
