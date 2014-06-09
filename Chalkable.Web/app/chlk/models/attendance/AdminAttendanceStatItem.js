REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AdminAttendanceStatItem*/
    CLASS(
        'AdminAttendanceStatItem', [
            [ria.serialize.SerializeProperty('absencescount')],
            Number, 'absentCount',

            [ria.serialize.SerializeProperty('excusedcount')],
            Number, 'excusedCount',

            [ria.serialize.SerializeProperty('latescount')],
            Number, 'lateCount',

            String, 'summary',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
