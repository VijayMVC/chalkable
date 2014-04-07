REQUIRE('chlk.models.attendance.ClassAttendance');
//REQUIRE('chlk.converters.attendance.AttendanceLevelToTypeConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ClassAttendanceWithSeatPlace*/
    CLASS(
        'ClassAttendanceWithSeatPlace', [
            chlk.models.attendance.ClassAttendance, 'info',

            Number, 'index',

            Number, 'column',

            Number, 'row'
        ]);
});
