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

            [[chlk.models.grading.GradingTeacherClassSummaryViewData, Number, Number]],
            ArrayOf(chlk.models.grading.StudentGradingViewData), function getPreparedStudents(item, width_, interval_){
                var students = item.getAllStudents(), well = item.getWell(), trouble = item.getTrouble();
                var width = width_ || 670, maxScore = 100;
                var interval = interval_ || 0;
                var lastRight = - 2*interval-10, avg;
                students.forEach(function(student){
                    avg = student.getAvg();
                    if (avg > maxScore)
                        maxScore = Math.ceil(avg);
                });
                item.setMaxScore(maxScore);
                for(var i = students.length - 1; i >= 0; i--){
                    var student = students[i];
                    avg = student.getAvg();
                    var right = Math.floor(width - avg * width / maxScore)-10;
                    if(interval && right - lastRight < interval)
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
