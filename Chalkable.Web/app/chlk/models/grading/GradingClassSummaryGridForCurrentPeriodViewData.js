REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.grading.AlternateScore');
REQUIRE('chlk.models.grading.AvgComment');
REQUIRE('chlk.models.grading.AlphaGrade');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridForCurrentPeriodViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            [ria.serialize.SerializeProperty('gradingperiods')],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.serialize.SerializeProperty('classannouncementtypes')],
            ArrayOf(chlk.models.common.NameId), 'classAnnouncementTypes',

            ArrayOf(chlk.models.standard.Standard), 'standards',

            Boolean, 'ableEdit',

            Boolean, 'hasAccessToLE',

            [ria.serialize.SerializeProperty('currentgradinggrid')],
            chlk.models.grading.ShortGradingClassSummaryGridItems, 'currentGradingGrid',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            Boolean, 'inProfile',

            chlk.models.id.ClassId, 'classId'
        ]);
});