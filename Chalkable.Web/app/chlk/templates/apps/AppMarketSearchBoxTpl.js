REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.apps.AppMarketApplication');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppMarketSearchBoxTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppMarketSearch.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketApplication)],
        'AppMarketSearchBoxTpl', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.PictureId, 'smallPictureId'
        ])
});

