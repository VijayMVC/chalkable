REQUIRE('chlk.templates.grading.GradingClassStandardsTpl');
REQUIRE('chlk.activities.grading.BaseGradingPage');
REQUIRE('chlk.templates.grading.GradingClassStandardItemTpl');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassStandardsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassStandardsTpl)],
        'GradingClassStandardsPage', EXTENDS(chlk.activities.grading.BaseGradingPage), [

            OVERRIDE, function getGradingItems_(model){
                return model.getSummaryPart().getItems();
            }

        ]);
});