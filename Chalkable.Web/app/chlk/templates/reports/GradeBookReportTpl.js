REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.GradeBookReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.GradeBookReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/GradeBookReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.GradeBookReportViewData)],
        'GradeBookReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [


            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.BaseAnnouncementViewData), 'announcements'
        ])
});