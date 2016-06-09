REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AbsentMonthStat*/
    CLASS(
        'AbsentMonthStat', [
            [ria.serialize.SerializeProperty('absentclasses')],
            Array, 'absentClasses',

            chlk.models.common.ChlkDate, 'month'
        ]);
});
