REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.grading.GradingStatsByDateViewData');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingStudentSummaryViewData*/
    CLASS(
        'GradingStudentSummaryViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements',

            [ria.serialize.SerializeProperty('totalavgperdate')],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'totalAvgPerDate',

            [ria.serialize.SerializeProperty('peersavgperdate')],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'peersAvgPerDate'
        ]);
});
