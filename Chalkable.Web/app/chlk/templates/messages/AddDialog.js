REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.templates.messages.RecipientAutoComplete');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.AddDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/AddDialog.jade')],
        [ria.templates.ModelBind(chlk.models.messages.Message)],
        'AddDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.MessageId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'subject',

            [ria.templates.ModelPropertyBind],
            String, 'body',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Person, 'sender',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Person, 'recipient',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'sent'
        ])
});
