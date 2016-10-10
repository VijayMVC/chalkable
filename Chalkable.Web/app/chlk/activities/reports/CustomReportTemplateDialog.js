REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.CustomReportTemplateDialogTpl');

NAMESPACE('chlk.activities.reports', function () {
    /** @class chlk.activities.reports.CustomReportTemplateDialog */
    CLASS(
        [ria.mvc.ActivityGroup('ReportTemplateDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.reports.CustomReportTemplateDialogTpl)],
        'CustomReportTemplateDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
});
