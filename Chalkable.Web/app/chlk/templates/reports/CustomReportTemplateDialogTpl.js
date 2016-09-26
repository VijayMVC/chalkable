REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.reports.CustomReportTemplateFormViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.CustomReportTemplateDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/CustomReportTemplateDialog.jade')],
        [ria.templates.ModelBind(chlk.models.reports.CustomReportTemplateFormViewData)],
        'CustomReportTemplateDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.reports.CustomReportTemplate, 'reportTemplate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.reports.CustomReportTemplate), 'headers',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.reports.CustomReportTemplate), 'footers',
        ]);
});