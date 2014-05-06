REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentAttendanceHoverBoxItem*/
    CLASS(
        'StudentAttendanceHoverBoxItem', [
            Number, 'value',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className'
        ]);
});
