REQUIRE('chlk.templates.grading.StandardsPopupTpl');
REQUIRE('chlk.models.grading.StandardsPopupViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.StandardsPopupItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStandardPopupItems.jade')],
        [ria.templates.ModelBind(chlk.models.grading.StandardsPopupViewData)],
        'StandardsPopupItemsTpl', EXTENDS(chlk.templates.grading.StandardsPopupTpl), [
        ])
});