REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.FinalGradesViewData');
REQUIRE('chlk.models.grading.AvgComment');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.FinalGradesTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/FinalGrades.jade')],
        [ria.templates.ModelBind(chlk.models.grading.FinalGradesViewData)],
        'FinalGradesTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradingPeriodFinalGradeViewData, 'currentFinalGrade',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToLE',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEditDirectValue',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inProfile',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId'
        ]);
});
