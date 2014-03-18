REQUIRE('chlk.models.grading.StudentWithAvg');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridItems*/
    CLASS(
        'GradingClassSummaryGridItems', [
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'announcements',
            [ria.serialize.SerializeProperty('markingperiod')],
            chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',
            Number, 'avg'
        ]);
});
