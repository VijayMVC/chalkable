REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.storage.Blobs');

NAMESPACE('chlk.activities.storage', function () {

    /** @class chlk.activities.storage.StorageBlobsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.storage.Blobs)],
        'StorageBlobsPage', EXTENDS(ria.mvc.TemplateActivity), [ ]);
});