REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.grading.GradingClassSummaryGridViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridViewData)],
        'GradingClassSummaryGridTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.Mapping, 'gradingStyleMapper',

            Array, function getAutoFillValues(){
                return ['A+', 'A', 'A-','B+', 'B', 'B-','C+', 'C', 'C-','D+', 'D', 'D-','F',
                    'F (fill all)','A (fill all)','B (fill all)','C (fill all)','D (fill all)',
                    'Complete', 'Complete (fill all)', 'Exempt', 'Exempt (fill all)', 'Fail',
                    'Fail (fill all)', 'Incomplete', 'Incomplete (fill all)', 'Late', 'Late (fill all)',
                    'Pass', 'Pass (fill all)'
                ];
            },

            String, function getTooltipText(item){
                var res = [];
                if(item.isLate())
                    res.push(Msg.Late);
                if(item.isIncomplete())
                    res.push(Msg.Incomplete);
                if(item.isAbsent())
                    res.push(Msg.Student_marked_absent);
                if(!res.length)
                    return '';
                return res.join('<hr>');
            },

            String, function getAlertClass(item){
                if(item.isLate()){
                    if(!item.isIncomplete())
                        return Msg.Late.toLowerCase();
                    if(item.isIncomplete())
                        return Msg.Multiple.toLowerCase();
                }
                else
                    if(item.isIncomplete())
                        return Msg.Incomplete.toLowerCase();
                    else
                        if(item.isAbsent())
                            return Msg.Absent.toLowerCase();
                return '';
            },

            Object, function getNormalValue(item){
                var value = item.getGradeValue();
                if(item.isDropped())
                    return Msg.Dropped;
                if(item.isExempt())
                    return Msg.Exempt;
                return (value >= 0) ? value : '';
            }
        ]);
});
