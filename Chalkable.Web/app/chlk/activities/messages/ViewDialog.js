REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.messages.ViewDialog');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.ViewDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('MessagePopup')],
        [ria.mvc.TemplateBind(chlk.templates.messages.ViewDialog)],
        'ViewDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
        ]);
});

