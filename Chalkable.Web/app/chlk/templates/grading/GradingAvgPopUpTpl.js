REQUIRE('chlk.templates.grading.StudentAverageTpl');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.GradingAvgPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingAvgPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortStudentAverageInfo)],
        'GradingAvgPopUpTpl', EXTENDS(chlk.templates.grading.StudentAverageTpl), [])
});