REQUIRE('chlk.models.grading.GradingClassSummaryItem');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItems*/
    CLASS(
        'GradingClassSummaryItems', [
            [ria.serialize.SerializeProperty('byannouncementtypes')],
            ArrayOf(chlk.models.grading.GradingClassSummaryItem), 'byAnnouncementTypes',
            [ria.serialize.SerializeProperty('markingperiod')],
            chlk.models.schoolYear.MarkingPeriod, 'markingPeriod',
            Number, 'avg'
        ]);
});
