REQUIRE('chlk.templates.common.SimpleObjectTpl');
REQUIRE('chlk.models.common.SimpleObject');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassSummaryGridAvgsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryAvgs.jade')],
        [ria.templates.ModelBind(chlk.models.common.SimpleObject)],
        'ShortGradingClassSummaryGridAvgsTpl', EXTENDS(chlk.templates.common.SimpleObjectTpl), [
            String, function displayAvgName(studentAverage){
                return studentAverage && studentAverage.isgradingperiodaverage
                    ? Msg.Avg : studentAverage.averagename;
            },

            Boolean, function withCodes(average){
                var codes = average.codes;
                if(!codes || !codes.length)
                    return false;
                var p = false;
                codes.forEach(function(item){
                    if(item.gradingcomment)
                        p = true;
                });
                return p
            },

            function getNumericAvg(average){
                return average.enteredavg || average.enteredavg == 0 ? average.enteredavg  : average.calculatedavg
            },

            function getAlphaGrade(average){
                return average.enteredalphagrade || average.calculatedalphagrade
            },

            String, function displayGrade(grade_){
                var isRoundDisplayAverage = this.getModel().getValue().rounddisplayedaverages;
                return grade_ || grade_ == 0 ? grade_.toFixed(isRoundDisplayAverage ? 0 : 2) : '';
            },


            String, function displayAvgGradeValue(average, isAbleDisplayAlphaGrades_, original_, noText_){
                if(average.isexempt && !original_ && !noText_)
                    return Msg.Exempt;
                var alphaGrade = original_ ? average.calculatedalphagrade : this.getAlphaGrade(average);
                var res = this.displayGrade(original_ ? average.calculatedavg : this.getNumericAvg(average));
                if(res && this.getNumericAvg(average) != 0 && isAbleDisplayAlphaGrades_ && alphaGrade && alphaGrade.trim() != ''){
                    res += '(' + alphaGrade + ')';
                }
                return res;
            }
        ]);
});
