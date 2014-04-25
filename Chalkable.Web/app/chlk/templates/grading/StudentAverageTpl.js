REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.StudentAverageTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/StudentAverage.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortStudentAverageInfo)],
        'StudentAverageTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'averageId',

            [ria.templates.ModelPropertyBind],
            Number, 'calculatedAvg',

            [ria.templates.ModelPropertyBind],
            Number, 'enteredAvg',

            [ria.templates.ModelPropertyBind],
            String, 'calculatedAlphaGrade',

            [ria.templates.ModelPropertyBind],
            String, 'enteredAlphaGrade',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            Boolean, 'ableDisplayAlphaGrades'
        ]);
});
