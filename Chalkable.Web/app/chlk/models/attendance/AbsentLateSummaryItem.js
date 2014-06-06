REQUIRE('chlk.models.attendance.StudentSummaryItem');
REQUIRE('chlk.models.attendance.AttendanceSummaryStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AbsentLateSummaryItem*/
    CLASS(
        'AbsentLateSummaryItem', [
            [ria.serialize.SerializeProperty('classesstats')],
            ArrayOf(chlk.models.attendance.AttendanceSummaryStatItem), 'classesStats',

            ArrayOf(chlk.models.attendance.StudentSummaryItem), 'students'
        ]);
});
