REQUIRE('chlk.models.dbmaintenance.DbBackup');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.storage.DbMaintenance*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/dbmaintenance.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'DbMaintenance', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.dbmaintenance.DbBackup), 'items'
        ])
});