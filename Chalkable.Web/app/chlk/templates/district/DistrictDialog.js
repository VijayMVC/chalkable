REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.DistrictDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/district-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.district.District)],
        'DistrictDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.district.DistrictId, 'id',
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