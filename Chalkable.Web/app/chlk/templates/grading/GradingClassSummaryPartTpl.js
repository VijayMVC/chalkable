REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingClassSummaryPart');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingClassSummaryPartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingClassSummaryPart.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryPart)],
        'GradingClassSummaryPartTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassStandardsItems), 'items',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            [[chlk.models.grading.GradingClassStandardsItems]],
            String, function getGPAvgToolTipText(item){
                return item.getAvg() != null ? Msg.Avg + " " + item.getAvg().toFixed(2) : 'No grades yet';
            }
        ]);
});
