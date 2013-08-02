REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.apps.Apps');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.Apps)],
        'AppsListPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});