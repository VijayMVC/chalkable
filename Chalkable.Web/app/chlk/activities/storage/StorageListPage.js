REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.storage.Storages');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.StorageListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.storage.Storages)],
        'StorageListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});