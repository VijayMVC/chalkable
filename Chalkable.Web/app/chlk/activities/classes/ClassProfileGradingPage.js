REQUIRE('chlk.activities.grading.BaseGradingPage');
REQUIRE('chlk.templates.classes.ClassProfileGradingTpl');
REQUIRE('chlk.models.grading.GradingClassSummaryPart');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileGradingTpl)],
        'ClassProfileGradingPage', EXTENDS(chlk.activities.grading.BaseGradingPage), [
            OVERRIDE, function getGradingItems_(model){
                return model.getGradingPart().getGradingPerMp();
            }
        ]);
});