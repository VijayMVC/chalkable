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

            //[ria.templates.ModelPropertyBind],
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [ria.templates.ModelPropertyBind],
            Array, 'codes',

            /*Array, function getCodes(){
                var res = [{
                    headerid: 1,
                    headername: 'Q1 Com1',
                    gradingcomment: {
                        code: "PAS",
                        comment: "- Can be passively defiant",
                        id: 6
                    }
                }, {
                    headerid: 2,
                    headername: 'Q1 Com2',
                    gradingcomment: {
                        code: "DRT",
                        comment: "- Distracts others",
                        id: 7
                    }
                }];
                return res;
            },*/

            Boolean, 'ableDisplayAlphaGrades'
        ]);
});
