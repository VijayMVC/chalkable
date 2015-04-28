REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.ViewDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/ViewDialog.jade')],
        [ria.templates.ModelBind(chlk.models.messages.Message)],
        'ViewDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.MessageId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'subject',

            [ria.templates.ModelPropertyBind],
            String, 'body',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'sender',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'recipient',

            [ria.templates.ModelPropertyBind],
            Boolean, 'replay',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'sent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inbox'
        ])
});

