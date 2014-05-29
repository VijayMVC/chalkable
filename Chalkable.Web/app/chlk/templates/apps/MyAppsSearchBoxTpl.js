REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.apps.AppMarketApplication');


NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.MyAppsSearchBoxTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/MyAppsSearch.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketApplication)],
        'MyAppsSearchBoxTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.PictureId, 'smallPictureId',


            [ria.templates.ModelPropertyBind],
            String, 'myAppsUrl',

            [ria.templates.ModelPropertyBind],
            String, 'url'
        ])
});

