REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingClassSummaryGridViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassStandardsGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridStandards.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridViewData)],
        'GradingClassStandardsGridTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), 'items',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            Object, function getNormalValue(item){
                return item.getGradeValue();
            }
        ]);
});
