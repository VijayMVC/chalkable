REQUIRE('chlk.templates.grading.StudentAverageTpl');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.StudentAverageInputTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingAvgInput.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortStudentAverageInfo)],
        'StudentAverageInputTpl', EXTENDS(chlk.templates.grading.StudentAverageTpl), []);
});
