REQUIRE('chlk.models.storage.Blob');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.storage.Blobs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/Blobs.jade')],
        [ria.templates.ModelBind(chlk.models.storage.BlobsListViewData)],
        'Blobs', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'items',
            [ria.templates.ModelPropertyBind],
            String, 'containerAddress'
        ])
});