REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryForCurrentPeriodViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            [ria.serialize.SerializeProperty('gradingperiods')],
            ArrayOf(chlk.models.common.NameId), 'gradingPeriods',

            [ria.serialize.SerializeProperty('currentgradingbox')],
            chlk.models.grading.GradingClassSummaryItems, 'currentGradingBox'
        ]);
});