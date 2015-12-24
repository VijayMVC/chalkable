REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes.grading', function () {

    /** @class chlk.templates.classes.grading.ClassProfileGradingStandardsGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/grading/ClassProfileGradingStandardsGrid.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileGradingStandardsGridTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});