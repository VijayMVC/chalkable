REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.grading.GradingStudentSummaryViewData');
REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingStudentSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStudentSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingStudentSummaryViewData)],
        'GradingStudentSummaryTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'totalAvgPerDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'peersAvgPerDate'
        ]);
});
