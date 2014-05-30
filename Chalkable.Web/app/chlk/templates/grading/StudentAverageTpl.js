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
            Boolean, 'mayBeExempt',

            [ria.templates.ModelPropertyBind],
            Boolean, 'exempt',

            [ria.templates.ModelPropertyBind],
            String, 'codesString',

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

            //[ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            Array, 'codes',

            Boolean, function withCodes(){
                var codes = this.getCodes();
                if(!codes || !codes.length)
                    return false;
                var p = false;
                codes.forEach(function(item){
                    if(item.gradingcomment)
                        p = true;
                });
                return p
            },

            Boolean, 'ableDisplayAlphaGrades'
        ]);
});
