REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.id.CustomReportTemplateId');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.CustomReportTemplateDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/CustomReportTemplateDialog.jade')],
        [ria.templates.ModelBind(chlk.models.reports.CustomReportTemplate)],
        'CustomReportTemplateDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.CustomReportTemplateId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'layout',

            [ria.templates.ModelPropertyBind],
            String, 'style',

            [ria.templates.ModelPropertyBind],
            Object, 'icon'
        ]);
});