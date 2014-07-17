REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.DailyAttendanceId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.DailyAttendance*/
    CLASS(
        'DailyAttendance', [
            chlk.models.id.DailyAttendanceId, 'id',

            chlk.models.common.ChlkDate, 'date',

            [ria.serialize.SerializeProperty('timein')],
            chlk.models.common.ChlkTime, 'timeIn',

            [ria.serialize.SerializeProperty('timeout')],
            chlk.models.common.ChlkTime, 'timeOut',

            chlk.models.common.ChlkTime, 'arrival',

            chlk.models.people.User, 'Person'
        ]);
});
