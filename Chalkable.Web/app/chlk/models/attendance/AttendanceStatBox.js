REQUIRE('chlk.models.attendance.AttendanceStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceStatBox*/
    CLASS(
        'AttendanceStatBox', [
            [ria.serialize.SerializeProperty('displaynumber')],
            Number, 'displayNumber',

            ArrayOf(chlk.models.attendance.AttendanceStatItem), 'stat'
        ]);
});
