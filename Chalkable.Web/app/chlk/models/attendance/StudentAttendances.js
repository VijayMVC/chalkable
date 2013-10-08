REQUIRE('chlk.models.attendance.AttendanceByType');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentAttendances*/
    CLASS(
        'StudentAttendances', [
            [ria.serialize.SerializeProperty('attendancetotalpertype')],
            ArrayOf(chlk.models.attendance.AttendanceByType), 'attendanceTotalPerType',

            chlk.models.people.User, 'student'
        ]);
});
