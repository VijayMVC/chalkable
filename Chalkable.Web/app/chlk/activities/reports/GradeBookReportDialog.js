REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.GradeBookReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.GradeBookReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.GradeBookReportTpl)],
        'GradeBookReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[]);
});