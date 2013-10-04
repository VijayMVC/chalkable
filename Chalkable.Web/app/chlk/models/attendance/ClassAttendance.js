REQUIRE('chlk.models.id.ClassAttendanceId');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.common.AttendanceTypeEnum*/
    ENUM(
        'AttendanceTypeEnum', {
            NA: 1,
            PRESENT: 2,
            EXCUSED: 4,
            ABSENT: 8,
            LATE: 16
        });

    /** @class chlk.models.attendance.ClassAttendance*/
    CLASS(
        'ClassAttendance', [
            chlk.models.id.ClassAttendanceId, 'id',

            [ria.serialize.SerializeProperty('classpersonid')],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.serialize.SerializeProperty('classperiodid')],
            chlk.models.id.ClassPeriodId, 'classPeriodId',

            chlk.models.common.ChlkDate, 'date',

            Number, 'type',

            chlk.models.period.Period, 'period',

            chlk.models.people.User, 'student',

            [ria.serialize.SerializeProperty('attendancereasonid')],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.serialize.SerializeProperty('attendancereason')],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            String, 'submitType',

            String, 'attendanceReasonDescription'
        ]);
});
