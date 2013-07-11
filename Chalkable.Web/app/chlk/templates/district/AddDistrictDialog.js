REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.AddDistrictDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/add-district-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.district.District)],
        'AddDistrictDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'name',
            [ria.templates.ModelBind],
            String, 'sisUrl',
            [ria.templates.ModelBind],
            String, 'dbName',
            [ria.templates.ModelBind],
            String, 'sisUserName',
            [ria.templates.ModelBind],
            String, 'sisPassword',
            [ria.templates.ModelBind],
            String, 'sisSystemType'
        ])
});