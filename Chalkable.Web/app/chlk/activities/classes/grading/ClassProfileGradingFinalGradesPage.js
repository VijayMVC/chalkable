REQUIRE('chlk.activities.grading.FinalGradesPage');
REQUIRE('chlk.templates.classes.grading.ClassProfileGradingFinalGradesTpl');

NAMESPACE('chlk.activities.classes.grading', function () {

    /** @class chlk.activities.classes.grading.ClassProfileGradingFinalGradesPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.grading.ClassProfileGradingFinalGradesTpl)],
        'ClassProfileGradingFinalGradesPage', EXTENDS(chlk.activities.grading.FinalGradesPage), [
            OVERRIDE, function prepareGradingModel_(model){
                return model.getClazz().getGradingPart();
            }
        ]);
});