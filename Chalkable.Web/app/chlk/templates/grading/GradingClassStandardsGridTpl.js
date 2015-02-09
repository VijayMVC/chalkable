REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassStandardsGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridStandards.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData)],
        'GradingClassStandardsGridTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradingClassSummaryGridItems.OF(chlk.models.standard.StandardGradings), 'currentGradingGrid',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ]);
});
