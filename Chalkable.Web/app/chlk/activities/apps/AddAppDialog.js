REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AddAppDialog');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AddAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddAppDialog)],
        'AddAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [ ]);
});