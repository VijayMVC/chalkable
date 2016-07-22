NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.TotalAttendanceViewData*/
    CLASS(
        'TotalAttendanceViewData', [
            [ria.serialize.SerializeProperty('latecount')],
            Number, 'lateCount',

            [ria.serialize.SerializeProperty('presentcount')],
            Number, 'presentCount',

            [ria.serialize.SerializeProperty('absentcount')],
            Number, 'absentCount',

            [ria.serialize.SerializeProperty('dayscount')],
            Number, 'daysCount'
        ]);
});
