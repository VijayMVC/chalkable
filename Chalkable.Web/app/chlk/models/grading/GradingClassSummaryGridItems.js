REQUIRE('chlk.models.grading.StudentWithAvg');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridItems*/
    CLASS(
        'GradingClassSummaryGridItems', [
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',
            [ria.serialize.SerializeProperty('markingperiod')],
            chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',
            Number, 'avg'
        ]);
});
