REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.SubmitLunchCountViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.LunchCountSubmitFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/LunchCountSubmitForm.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitLunchCountViewData)],
        'LunchCountSubmitFormTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableDownload'

    ]);
});