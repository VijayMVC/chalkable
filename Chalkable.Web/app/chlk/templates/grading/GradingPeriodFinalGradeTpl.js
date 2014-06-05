REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingPeriodFinalGradeViewData');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingPeriodFinalGradeTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingPeriodFinalGrade.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingPeriodFinalGradeViewData)],
        'GradingPeriodFinalGradeTpl',  EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentFinalGradeViewData), 'studentFinalGrades',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.StudentAverageInfo, 'currentAverage',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'averages',

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',

            [ria.templates.ModelPropertyBind],
            Boolean, 'avgChanged'
        ]);
});
