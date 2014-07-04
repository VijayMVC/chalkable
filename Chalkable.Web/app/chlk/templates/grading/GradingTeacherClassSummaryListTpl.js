REQUIRE('chlk.models.grading.GradingTeacherClassSummaryViewDataList');
REQUIRE('chlk.templates.common.PageWithClasses');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingTeacherClassSummaryListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingTeacherClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingTeacherClassSummaryViewDataList)],
        'GradingTeacherClassSummaryListTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingTeacherClassSummaryViewData), 'items',

            [[ArrayOf(chlk.models.grading.StudentGradingViewData), ArrayOf(chlk.models.grading.StudentGradingViewData),
                ArrayOf(chlk.models.grading.StudentGradingViewData), Number, Number]],
            ArrayOf(chlk.models.grading.StudentGradingViewData), function getPreparedStudents(students, well, trouble, width_, interval_){
                var width = width_ || 670;
                var interval = interval_ || 5;
                var lastRight = - 2*interval-10;
                for(var i = students.length - 1; i >= 0; i--){
                    var student = students[i];
                    var right = Math.floor(width - student.getAvg() * width / 100)-10;
                    if(right - lastRight < interval)
                        right = lastRight + interval;
                    lastRight = right;
                    student.setRight(right);
                    if((well || []).filter(function(item){return item.getId() == student.getId()}).length)
                        student.setWellTroubleType(chlk.models.grading.StudentWellTroubleEnum.WELL);
                    else
                        if((trouble || []).filter(function(item){return item.getId() == student.getId()}).length)
                            student.setWellTroubleType(chlk.models.grading.StudentWellTroubleEnum.TROUBLE);
                        else
                            student.setWellTroubleType(chlk.models.grading.StudentWellTroubleEnum.NORMALL);
                }
                return students;
            }
        ]);
});
