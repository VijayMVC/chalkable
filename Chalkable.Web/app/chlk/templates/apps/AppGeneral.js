REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.AppGeneral*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/AppGeneralInfo.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData)],
        'AppGeneral', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'appName',
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppState, 'appState',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'appId'

        ])
});