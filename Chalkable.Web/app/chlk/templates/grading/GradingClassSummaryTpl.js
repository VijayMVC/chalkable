REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingClassSummary');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummary)],
        'GradingClassSummaryTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

            [ria.templates.ModelPropertyBind],
            String, 'action',

            [ria.templates.ModelPropertyBind],
            String, 'gridAction'
        ]);
});
