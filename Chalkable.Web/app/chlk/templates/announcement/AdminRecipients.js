REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.announcement.AdminRecipients');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AdminRecipients*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminRecipients.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AdminRecipients)],
        'AdminRecipients', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AdminRecipientButton), 'recipientButtonsInfo',

            [ria.templates.ModelPropertyBind],
            Object, 'recipientsData'
        ])
});