REQUIRE('chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.StandardsInputTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/StandardsInput.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardGrading)],
        'StandardsInputTpl', EXTENDS(chlk.templates.grading.TeacherClassGradingGridStandardsItemTpl), [])
});