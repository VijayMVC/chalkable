REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradingStandardsPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStandardsPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardGrading)],
        'GradingStandardsPopUpTpl', EXTENDS(chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl), [])
});