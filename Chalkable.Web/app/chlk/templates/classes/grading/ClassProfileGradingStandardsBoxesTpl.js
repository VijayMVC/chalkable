REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes.grading', function () {

    /** @class chlk.templates.classes.grading.ClassProfileGradingStandardsBoxesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/grading/ClassProfileGradingStandardsBoxes.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileGradingStandardsBoxesTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});