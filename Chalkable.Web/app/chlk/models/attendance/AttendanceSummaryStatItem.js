REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.attendance.AttendanceStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceSummaryStatItem*/
    CLASS(
        'AttendanceSummaryStatItem', [
            [ria.serialize.SerializeProperty('daystats')],
            ArrayOf(chlk.models.attendance.AttendanceStatItem), 'dayStats',

            [ria.serialize.SerializeProperty('class')],
            chlk.models.classes.Class, 'clazz'
        ]);
});
