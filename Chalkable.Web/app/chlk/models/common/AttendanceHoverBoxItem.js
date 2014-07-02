REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.AttendanceHoverBoxItem*/
    CLASS(
        'AttendanceHoverBoxItem', [
            Number, 'absences',

            [ria.serialize.SerializeProperty('class')],
            chlk.models.classes.Class, 'clazz'
        ]);
});
