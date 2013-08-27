REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.messages.AddDialog');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.AddDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.messages.AddDialog)],
        'AddDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
        ]);
});
