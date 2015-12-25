REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes.grading', function () {

    /** @class chlk.templates.classes.grading.ClassProfileGradingFinalGradesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/grading/ClassProfileGradingFinalGrades.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileGradingFinalGradesTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ])
});