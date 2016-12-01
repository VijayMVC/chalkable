NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceByType*/
    CLASS(
        'AttendanceByType', [
            [ria.serialize.SerializeProperty('attendancecount')],
            Number, 'attendanceCount',

            Number, 'type'
        ]);
});
