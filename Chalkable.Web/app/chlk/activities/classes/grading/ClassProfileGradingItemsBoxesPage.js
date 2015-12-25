REQUIRE('chlk.activities.grading.GradingClassSummaryPage');
REQUIRE('chlk.templates.classes.grading.ClassProfileGradingItemsBoxesTpl');

NAMESPACE('chlk.activities.classes.grading', function () {

    /** @class chlk.activities.classes.grading.ClassProfileGradingItemsBoxesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.grading.ClassProfileGradingItemsBoxesTpl)],
        'ClassProfileGradingItemsBoxesPage', EXTENDS(chlk.activities.grading.GradingClassSummaryPage), [
            OVERRIDE, function prepareGradingModel_(model){
                return model.getClazz().getGradingPart();
            }
        ]);
});