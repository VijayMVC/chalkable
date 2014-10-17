REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.TeacherClassGradingSummaryItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingSummaryItem.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryItems)],
        'TeacherClassGradingSummaryItemTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'autoUpdate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassSummaryItem), 'byAnnouncementTypes',

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            Number, 'avg'
        ]);
});
