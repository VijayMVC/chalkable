REQUIRE('chlk.models.grading.GradingStatsByDateViewData');
REQUIRE('chlk.models.grading.AnnTypeGradeStatsViewData');
REQUIRE('chlk.models.grading.GradingClassSummaryPart');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingStudentClassSummaryViewData*/
    CLASS(
        'GradingStudentClassSummaryViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            [ria.serialize.SerializeProperty('currentmarkingperiod')],
            chlk.models.schoolYear.MarkingPeriod, 'currentMarkingPeriod',

            [ria.serialize.SerializeProperty('avgperdate')],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'avgPerDate',

            [ria.serialize.SerializeProperty('anntypesgradestats')],
            ArrayOf(chlk.models.grading.AnnTypeGradeStatsViewData), 'annTypesGradeStats',

            [ria.serialize.SerializeProperty('gradingpermp')],
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items',

            chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

            chlk.models.classes.ClassForTopBar, 'clazz'
        ])
});
