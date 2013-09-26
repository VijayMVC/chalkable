REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceList*/
    CLASS(
        'AttendanceList', [
            String, 'classPersonIds',

            String, 'classPeriodIds',

            String, 'attendanceTypes',

            String, 'attReasons',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
