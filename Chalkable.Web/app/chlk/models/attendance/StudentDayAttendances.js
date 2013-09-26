REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attendance.DailyAttendance');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.Popup');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentDayAttendances*/
    CLASS(
        'StudentDayAttendances', EXTENDS(chlk.models.Popup), [
            ArrayOf(chlk.models.attendance.ClassAttendance), 'attendances',

            chlk.models.people.User, 'student',

            [ria.serialize.SerializeProperty('dailyattendance')],
            chlk.models.attendance.DailyAttendance, 'dailyAttendance',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            Boolean, 'ableEdit'
        ]);
});
