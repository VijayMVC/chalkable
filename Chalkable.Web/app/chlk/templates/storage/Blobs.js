REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.storage.Blob');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.storage.Blobs*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/Blobs.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Blobs', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.storage.Blob), 'items'
        ])
});