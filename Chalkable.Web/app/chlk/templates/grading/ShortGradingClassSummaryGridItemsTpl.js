REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryItem.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortGradingClassSummaryGridItems)],
        'ShortGradingClassSummaryGridItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.NameId, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            Boolean, 'autoUpdate',

            [ria.templates.ModelPropertyBind],
            Number, 'rowIndex',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayStudentAverage',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayTotalPoints',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            ArrayOf(Number), 'totalPoints',

            [[chlk.models.grading.StudentAverageInfo]],
            String, function displayAvgName(studentAverage){
                return studentAverage && studentAverage.isGradingPeriodAverage()
                    ? Msg.Avg : studentAverage.getAverageName();
            },

            [[Number]],
            String, function displayGrade(grade){
                return grade ? grade.toFixed(2) : '';
            },

            [[chlk.models.grading.ShortStudentAverageInfo]],
            String, function displayAvgGradeValue(averageInfo){
                var alphaGrade = averageInfo.getAlphaGradeValue();
                var res = this.displayGrade(averageInfo.getNumericValue());
                if(res && this.isAbleDisplayAlphaGrades() && alphaGrade && alphaGrade.trim() != ''){
                    res += '(' + alphaGrade + ')';
                }
                return res;
            }
        ]);
});
