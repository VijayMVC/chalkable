REQUIRE('chlk.models.attendance.StudentSummaryItem');
REQUIRE('chlk.models.attendance.AttendanceStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AbsentLateSummaryItem*/
    CLASS(
        'AbsentLateSummaryItem', [
            ArrayOf(chlk.models.attendance.AttendanceStatItem), 'stat',

            ArrayOf(chlk.models.attendance.StudentSummaryItem), 'students'
        ]);
});
