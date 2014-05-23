REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceStatItem*/
    CLASS(
        'AttendanceStatItem', [
            [ria.serialize.SerializeProperty('studentcount')],
            Number, 'studentCount',

            String, 'summary',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
