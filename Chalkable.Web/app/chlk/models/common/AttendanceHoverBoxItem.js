REQUIRE('chlk.converters.attendance.ClassAttendanceIdToNameConverter');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.AttendanceHoverBoxItem*/
    CLASS(
        'AttendanceHoverBoxItem', [
            Number, 'type',

            [ria.serialize.SerializeProperty('attendancecount')],
            Number, 'attendanceCount'
        ]);
});
