REQUIRE('chlk.templates.grading.GradeLevel');
REQUIRE('chlk.models.grading.GradeLevelForTopBar');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradeLevelForTopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradeLevelForTopBar.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradeLevelForTopBar)],
        'GradeLevelForTopBar', EXTENDS(chlk.templates.grading.GradeLevel), [
            [ria.templates.ModelPropertyBind],
            String, 'controller',

            [ria.templates.ModelPropertyBind],
            String, 'action',

            [ria.templates.ModelPropertyBind],
            Array, 'params',

            [ria.templates.ModelPropertyBind],
            Boolean, 'pressed'
        ]);
});
