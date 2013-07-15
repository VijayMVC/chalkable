REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.AddDistrictDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/add-district-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.district.District)],
        'AddDistrictDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'sisUrl',
            [ria.templates.ModelPropertyBind],
            String, 'dbName',
            [ria.templates.ModelPropertyBind],
            String, 'sisUserName',
            [ria.templates.ModelPropertyBind],
            String, 'sisPassword',
            [ria.templates.ModelPropertyBind],
            Number, 'sisSystemType'
        ])
});