REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingStudentSummaryTpl');
REQUIRE('chlk.templates.grading.GradingStudentSummaryChartTpl');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingStudentSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingStudentSummaryTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingStudentSummaryChartTpl, 'chart-update', '.chart-container-1', ria.mvc.PartialUpdateRuleActions.Replace)],
        'GradingStudentSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});