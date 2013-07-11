REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.storage.Storage');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.storage.Storages*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/Storages.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Storages', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.storage.Storage), 'items'
        ])
});