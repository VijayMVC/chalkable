REQUIRE('chlk.activities.grading.GradingClassStandardsGridPage');
REQUIRE('chlk.templates.classes.ClassProfileGradingTpl');

NAMESPACE('chlk.activities.classes.grading', function () {

    /** @class chlk.activities.classes.grading.ClassProfileGradingStandardsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileGradingTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileGradingStandardsGridPage', EXTENDS(chlk.activities.grading.GradingClassStandardsGridPage), [
            OVERRIDE, function prepareGradingModel_(model){
                return model.getClazz().getGradingPart();
            }
        ]);
});