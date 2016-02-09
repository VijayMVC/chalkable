REQUIRE('chlk.models.calendar.BaseCalendarMonthItem');
REQUIRE('chlk.models.attendance.ShortAttendanceSummary');

NAMESPACE('chlk.models.calendar.attendance', function () {
    "use strict";

    /** @class chlk.models.calendar.attendance.AttendanceCalendarMonthItem*/
    CLASS(
        'AttendanceCalendarMonthItem', EXTENDS(chlk.models.calendar.BaseCalendarMonthItem), [

            ArrayOf(chlk.models.attendance.ShortAttendanceSummary) , 'attendances'
    ]);
});
