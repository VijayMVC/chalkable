REQUIRE('chlk.templates.storage.Blobs');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.storage.DbMaintenance*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/DbMaintenance.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'DbMaintenance', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.storage.Blob), 'items'
        ])
});