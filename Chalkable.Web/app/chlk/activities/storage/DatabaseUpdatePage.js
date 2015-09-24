REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.storage.DatabaseUpdate');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.DatabaseUpdatePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.storage.DatabaseUpdate)],
        'DatabaseUpdatePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});