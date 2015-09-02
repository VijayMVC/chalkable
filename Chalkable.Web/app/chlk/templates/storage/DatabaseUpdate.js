REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.storage.DatabaseUpdate');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.DatabaseUpdate*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/DatabaseUpdate.jade')],
        [ria.templates.ModelBind(chlk.models.storage.DatabaseUpdate)],
        'DatabaseUpdate', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'masterSql',
            [ria.templates.ModelPropertyBind],
            String, 'schoolSql'
        ])
});