REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingClassSummary');
 
 NAMESPACE('chlk.templates.grading', function () {
     "use strict";
     /** @class chlk.templates.grading.GradingClassStandardsTpl*/
     CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingStandards.jade')],
         [ria.templates.ModelBind(chlk.models.grading.GradingClassSummary)],
         'GradingClassStandardsTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
             [ria.templates.ModelPropertyBind],
             chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

             [ria.templates.ModelPropertyBind],
             Boolean, 'inProfile',

             [ria.templates.ModelPropertyBind],
             chlk.models.id.ClassId, 'classId',

             [ria.templates.ModelPropertyBind],
             Boolean, 'hasAccessToLE'
         ]);
 });