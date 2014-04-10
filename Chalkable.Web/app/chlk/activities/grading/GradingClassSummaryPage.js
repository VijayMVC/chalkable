REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.activities.lib.TemplatePage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'GradingClassSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});