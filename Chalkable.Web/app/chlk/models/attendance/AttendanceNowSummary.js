REQUIRE('chlk.models.attendance.AdminAttendanceStatItem');
REQUIRE('chlk.models.attendance.AbsentNowStudent');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceNowSummary*/
    CLASS(
        'AttendanceNowSummary', [
            [ria.serialize.SerializeProperty('attendancestats')],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendanceStats',

            [ria.serialize.SerializeProperty('absentnowstudents')],
            ArrayOf(chlk.models.attendance.AbsentNowStudent), 'absentNowStudents',

            [ria.serialize.SerializeProperty('absentnowcount')],
            Number, 'absentNowCount',

            [ria.serialize.SerializeProperty('absentusually')],
            Number, 'absentUsually',

            [ria.serialize.SerializeProperty('avgofabsentsinyear')],
            Number, 'avgOfAbsentsInYear'
        ]);
});
