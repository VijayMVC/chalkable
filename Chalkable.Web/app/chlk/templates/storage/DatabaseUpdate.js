REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.storage.DatabaseUpdate');

NAMESPACE('chlk.templates.storage', function () {

    /** @class chlk.templates.DatabaseUpdate*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/storage/DatabaseUpdate.jade')],
        [ria.templates.ModelBind(chlk.models.storage.DatabaseUpdate)],
        'DatabaseUpdate', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            String, 'masterSql',
            [ria.templates.ModelBind],
            String, 'schoolSql'
        ])
});