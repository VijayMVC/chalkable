REQUIRE('chlk.models.attendance.AdminAttendanceStatItem');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceMpSummary*/
    CLASS(
        'AttendanceMpSummary', [
            [ria.serialize.SerializeProperty('attendancestats')],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendanceStats',

            [ria.serialize.SerializeProperty('absentandlatestudents')],
            ArrayOf(chlk.models.people.User), 'absentAndLateStudents',

            [ria.serialize.SerializeProperty('absentstudentscountavg')],
            Number, 'absentStudentsCountAvg'
        ]);
});
