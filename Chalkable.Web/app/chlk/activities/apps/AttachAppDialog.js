REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AttachAppDialog');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AttachAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AttachAppDialog)],
        'AttachAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
});