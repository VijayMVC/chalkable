REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attendance.AbsentMonthStat');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceSummary*/
    CLASS(
        'AttendanceSummary', [
            ArrayOf(chlk.models.people.User), 'trouble',

            ArrayOf(chlk.models.people.User), 'well',

            [ria.serialize.SerializeProperty('absentstat')],
            ArrayOf(chlk.models.attendance.AbsentMonthStat), 'absentStat' //todo: rename
        ]);
});
