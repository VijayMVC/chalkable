REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppScreenshots');
REQUIRE('chlk.models.apps.AppPrice');
REQUIRE('chlk.models.apps.AppPermission');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketDetails*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-market-details.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketApplication)],
        'AppMarketDetails', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'id',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.AppPermission), 'permissions',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppScreenshots, 'screenshotPictures',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppPrice, 'applicationPrice',

            [ria.templates.ModelPropertyBind],
            chlk.models.developer.DeveloperInfo, 'developerInfo'
        ])
});