REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.ClassPersonGradesByGradingPeriod');

NAMESPACE('chlk.templates.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.templates.student.StudentProfileGradingPeriodPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileGradingPeriodPart.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ClassPersonGradesByGradingPeriod)],
        'StudentProfileGradingPeriodPartTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.ClassPersonGradingInfo), 'studentGradings'
        ]);
});