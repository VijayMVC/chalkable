REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.storage.DatabaseUpdate');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.DatabaseUpdatePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.storage.DatabaseUpdate)],
        'DatabaseUpdatePage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});