REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.attendance.ClassAttendanceWithSeatPlace');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.SeatingChart*/
    CLASS(
        'SeatingChart', EXTENDS(chlk.models.common.PageWithClasses), [
            Number, 'columns',

            Number, 'rows',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('notseatingstudents')],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'notSeatingStudents',

            [ria.serialize.SerializeProperty('seatinglist')],
            ArrayOf(ArrayOf(chlk.models.attendance.ClassAttendanceWithSeatPlace)), 'seatingList',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons'
        ]);
});
