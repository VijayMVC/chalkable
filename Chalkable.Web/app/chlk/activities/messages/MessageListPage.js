REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.messages.MessageList');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.MessageListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.messages.MessageList)],
        'MessageListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});