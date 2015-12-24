REQUIRE('chlk.activities.grading.GradingClassSummaryGridPage');
REQUIRE('chlk.templates.classes.grading.ClassProfileGradingSummaryGridTpl');

NAMESPACE('chlk.activities.classes.grading', function () {

    /** @class chlk.activities.classes.grading.ClassProfileGradingItemsGridPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.grading.ClassProfileGradingSummaryGridTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.GradingCommentsTpl, chlk.activities.lib.DontShowLoader(), '.grading-comments-list', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileGradingItemsGridPage', EXTENDS(chlk.activities.grading.GradingClassSummaryGridPage), [
            OVERRIDE, function prepareGradingModel_(model){
                return model.getClazz().getGradingPart();
            }
        ]);
});