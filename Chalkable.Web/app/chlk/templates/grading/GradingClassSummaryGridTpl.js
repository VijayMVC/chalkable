REQUIRE('chlk.templates.grading.GradingClassStandardsGridTpl');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData)],
        'GradingClassSummaryGridTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'classAnnouncementTypes',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standards',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.ShortGradingClassSummaryGridItems, 'currentGradingGrid',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

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
            }
        ]);
});
