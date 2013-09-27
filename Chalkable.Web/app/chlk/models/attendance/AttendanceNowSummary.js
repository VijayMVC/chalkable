REQUIRE('chlk.models.attendance.AdminAttendanceStatItem');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceNowSummary*/
    CLASS(
        'AttendanceNowSummary', [
            [ria.serialize.SerializeProperty('attendancestats')],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendanceStats',

            [ria.serialize.SerializeProperty('absentnowstudents')],
            ArrayOf(chlk.models.people.User), 'absentNowStudents',

            [ria.serialize.SerializeProperty('absentnowcount')],
            Number, 'absentNowCount',

            [ria.serialize.SerializeProperty('absentusually')],
            Number, 'absentUsually',

            [ria.serialize.SerializeProperty('avgofabsentsinyear')],
            Number, 'avgOfAbsentsInYear'
        ]);
});
