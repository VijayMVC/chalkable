REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.apps.Apps');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.apps.Apps)],
        'AppsListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});