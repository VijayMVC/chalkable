REQUIRE('chlk.activities.grading.GradingClassSummaryPage');
REQUIRE('chlk.templates.classes.ClassProfileGradingTpl');

NAMESPACE('chlk.activities.classes.grading', function () {

    /** @class chlk.activities.classes.grading.ClassProfileGradingStandardsBoxesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileGradingTpl)],
        'ClassProfileGradingStandardsBoxesPage', EXTENDS(chlk.activities.grading.GradingClassStandardsPage), [
            OVERRIDE, function prepareGradingModel_(model){
                return model.getClazz().getGradingPart();
            }
        ]);
});