REQUIRE('chlk.templates.SuccessTpl');

REQUIRE('chlk.models.Success');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.StudentAvgPopUpTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.Success)],
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/StudentAvgPopUp.jade')],
        'StudentAvgPopUpTpl', EXTENDS(chlk.templates.SuccessTpl), [])
});