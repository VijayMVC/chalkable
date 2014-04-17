REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryAvgs.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortGradingClassSummaryGridItems)],
        'ShortGradingClassSummaryGridAvgsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.NameId, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            Number, 'rowIndex',

            [ria.templates.ModelPropertyBind],
            Boolean, 'autoUpdate',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayStudentAverage',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayTotalPoints',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            ArrayOf(Number), 'totalPoints'
        ]);
});
