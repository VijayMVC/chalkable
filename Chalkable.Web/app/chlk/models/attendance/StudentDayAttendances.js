REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attendance.DailyAttendance');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.common.AttendanceDisciplinePopUp');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentDayAttendances*/
    CLASS(
        'StudentDayAttendances', EXTENDS(chlk.models.common.AttendanceDisciplinePopUp), [
            ArrayOf(chlk.models.attendance.ClassAttendance), 'attendances',

            chlk.models.attendance.ClassAttendance, 'allItem',

            [ria.serialize.SerializeProperty('dailyattendance')],
            chlk.models.attendance.DailyAttendance, 'dailyAttendance',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons'
        ]);
});
