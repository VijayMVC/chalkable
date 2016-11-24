REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.reports.CustomReportTemplateListTpl');

NAMESPACE('chlk.activities.reports', function () {

    /** @class chlk.activities.reports.CustomReportTemplateListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.reports.CustomReportTemplateListTpl)],
        'CustomReportTemplateListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});