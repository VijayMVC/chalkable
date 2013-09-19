REQUIRE('chlk.models.common.PageWithGrades');
REQUIRE('chlk.models.attendance.AttendanceNowSummary');
REQUIRE('chlk.models.attendance.AttendanceDaySummary');
REQUIRE('chlk.models.attendance.AttendanceMpSummary');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AdminAttendanceSummary*/
    CLASS(
        'AdminAttendanceSummary', EXTENDS(chlk.models.common.PageWithGrades), [
            [ria.serialize.SerializeProperty('nowattendancedata')],
            chlk.models.attendance.AttendanceNowSummary, 'nowAttendanceData',

            [ria.serialize.SerializeProperty('attendancebydaydata')],
            chlk.models.attendance.AttendanceDaySummary, 'attendanceByDayData',

            [ria.serialize.SerializeProperty('attendancebympdata')],
            chlk.models.attendance.AttendanceMpSummary, 'attendanceByMpData'
        ]);
});
