REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.reports.ReportCardRecipientsViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.ReportCardRecipientsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/ReportCardRecipients.jade')],
        [ria.templates.ModelBind(chlk.models.reports.ReportCardRecipientsViewData)],
        'ReportCardRecipientsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups'
        ])
});