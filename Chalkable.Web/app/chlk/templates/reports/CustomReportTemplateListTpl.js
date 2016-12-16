REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.CustomReportTemplateList');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.CustomReportTemplateListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/CustomReportTemplateList.jade')],
        [ria.templates.ModelBind(chlk.models.reports.CustomReportTemplateList)],
        'CustomReportTemplateListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates'
        ]);
});