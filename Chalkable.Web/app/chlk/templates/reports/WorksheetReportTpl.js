REQUIRE('chlk.templates.reports.GradeBookReportTpl');
REQUIRE('chlk.models.reports.GradeBookReportViewData');
REQUIRE('chlk.templates.reports.WorksheetReportGridTpl');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.WorksheetReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/WorksheetReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.GradeBookReportViewData)],
        'WorksheetReportTpl', EXTENDS(chlk.templates.reports.GradeBookReportTpl), [])
});