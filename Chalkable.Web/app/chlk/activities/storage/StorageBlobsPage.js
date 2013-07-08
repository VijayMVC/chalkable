REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.storage.Blobs');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.StorageBlobsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.storage.Blobs)],
        'StorageBlobsPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});