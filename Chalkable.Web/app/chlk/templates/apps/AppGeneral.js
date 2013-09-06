REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppGeneral*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-general-info.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData)],
        'AppGeneral', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'app',
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.Application, 'liveApp',
            [ria.templates.ModelPropertyBind],
            Boolean, 'isLiveApp',
            [ria.templates.ModelPropertyBind],
            String, 'appThumbnail'
        ])
});