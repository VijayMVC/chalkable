REQUIRE('chlk.models.attendance.AbsentLateSummaryItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceSummary*/
    CLASS(
        'AttendanceSummary', [
            chlk.models.attendance.AbsentLateSummaryItem, 'late',

            chlk.models.attendance.AbsentLateSummaryItem, 'absent'
        ]);
});
