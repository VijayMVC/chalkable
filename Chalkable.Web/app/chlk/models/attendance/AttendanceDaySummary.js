REQUIRE('chlk.models.attendance.AdminAttendanceStatItem');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceDaySummary*/
    CLASS(
        'AttendanceDaySummary', [
            [ria.serialize.SerializeProperty('attendancestats')],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendancesStats',

            [ria.serialize.SerializeProperty('studentsabsentwholeday')],
            ArrayOf(chlk.models.people.User), 'studentsAbsentWholeDay',

            [ria.serialize.SerializeProperty('studentscountabsentwholeday')],
            Number, 'studentsCountAbsentWholeDay',

            [ria.serialize.SerializeProperty('absentstudents')],
            ArrayOf(chlk.models.people.User), 'absentStudents',

            [ria.serialize.SerializeProperty('excusedstudents')],
            ArrayOf(chlk.models.people.User), 'excusedStudents',

            [ria.serialize.SerializeProperty('latestudents')],
            ArrayOf(chlk.models.people.User), 'lateStudents'
        ]);
});
