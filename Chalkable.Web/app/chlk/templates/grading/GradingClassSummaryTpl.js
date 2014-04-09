REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData)],
        'GradingClassSummaryTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [

        ]);
});
