REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.LessonPlanReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.LessonPlanReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/LessonPlanReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.LessonPlanReportViewData)],
        'LessonPlanReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'activityCategories',

            [ria.templates.ModelPropertyBind],
            Array, 'activityAttributes'
        ])
});