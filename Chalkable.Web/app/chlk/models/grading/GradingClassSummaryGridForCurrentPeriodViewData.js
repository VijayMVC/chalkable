REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridForCurrentPeriodViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            [ria.serialize.SerializeProperty('gradingperiods')],
            ArrayOf(chlk.models.common.NameId), 'gradingPeriods',

            [ria.serialize.SerializeProperty('currentgradinggrid')],
            chlk.models.grading.ShortGradingClassSummaryGridItems, 'currentGradingGrid',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores'
        ]);
});