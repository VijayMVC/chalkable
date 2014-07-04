REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.messages.MessageList');

NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.MessageList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/MessageList.jade')],
        [ria.templates.ModelBind(chlk.models.messages.MessageList)],
        'MessageList', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'messages',
            [ria.templates.ModelPropertyBind],
            Boolean, 'inbox',
            [ria.templates.ModelPropertyBind],
            String, 'role',
            [ria.templates.ModelPropertyBind],
            String, 'keyword',
            [ria.templates.ModelPropertyBind],
            Number, 'start'
        ])
});
