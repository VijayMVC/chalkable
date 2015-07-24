REQUIRE('chlk.models.id.DailyAttendanceId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.common.ChlkTime');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.DailyAttendance*/
    CLASS(
        'DailyAttendance', [
            chlk.models.id.DailyAttendanceId, 'id',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('arrivaltime')],
            chlk.models.common.ChlkTime, 'arrivalTime',

            [ria.serialize.SerializeProperty('checkintime')],
            chlk.models.common.ChlkTime, 'checkInTime',

            [ria.serialize.SerializeProperty('ischeckin')],
            Boolean, 'checkIn',

            [ria.serialize.SerializeProperty('periodattendances')],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'periodAttendances'
        ]);
});
