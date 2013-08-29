REQUIRE('chlk.models.id.ClassAttendanceId');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ClassAttendance*/
    CLASS(
        'ClassAttendance', [
            chlk.models.id.ClassAttendanceId, 'id',

            [ria.serialize.SerializeProperty('classpersonid')],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.serialize.SerializeProperty('classperiodid')],
            chlk.models.id.ClassPeriodId, 'classPeriodId'.

            chlk.models.common.ChlkDate, 'date',

            Number, 'type',

            chlk.models.period.Period, 'period',

            chlk.models.people.User, 'student',

            [ria.serialize.SerializeProperty('attendancereasonid')],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId'
        ]);
});
