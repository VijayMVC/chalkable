REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes.grading', function () {

    /** @class chlk.templates.classes.grading.ClassProfileGradingSummaryGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/grading/ClassProfileGradingSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileGradingSummaryGridTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});