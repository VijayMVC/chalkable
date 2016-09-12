REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.BirthdayReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.BirthdayReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/BirthdayReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.BirthdayReportViewData)],
        'BirthdayReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [])
});