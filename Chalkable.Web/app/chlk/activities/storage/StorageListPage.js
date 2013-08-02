REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.storage.Storages');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.StorageListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.storage.Storages)],
        'StorageListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});