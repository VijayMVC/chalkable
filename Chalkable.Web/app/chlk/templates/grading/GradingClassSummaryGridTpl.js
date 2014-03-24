REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.models.grading.GradingClassSummaryGridViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridViewData)],
        'GradingClassSummaryGridTpl', EXTENDS(chlk.templates.grading.GradingClassStandardsGridTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

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

            OVERRIDE, Object, function getNormalValue(item){
                var value = item.getGradeValue();
                if(item.isDropped())
                    return Msg.Dropped;
                if(item.isExempt())
                    return Msg.Exempt;
                return (value >= 0) ? value : '';
            }
        ]);
});
