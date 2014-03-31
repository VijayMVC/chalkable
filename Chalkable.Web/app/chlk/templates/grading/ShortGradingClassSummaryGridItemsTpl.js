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
            Number, 'avg'
        ]);
});
