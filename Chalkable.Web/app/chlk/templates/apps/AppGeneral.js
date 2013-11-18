REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppGeneral*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-general-info.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData)],
        'AppGeneral', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'draftAppId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'liveAppId',

            [ria.templates.ModelPropertyBind],
            String, 'appStatus',

            [ria.templates.ModelPropertyBind],
            String, 'appName',

            [ria.templates.ModelPropertyBind],
            Boolean, 'appLive',

            [ria.templates.ModelPropertyBind],
            Boolean, 'approved',

            [ria.templates.ModelPropertyBind],
            String, 'appThumbnail'
        ])
});