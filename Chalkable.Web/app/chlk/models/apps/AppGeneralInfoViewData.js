REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppGeneralInfoViewData*/
    CLASS(
        'AppGeneralInfoViewData', [
            String, 'appName',
            chlk.models.apps.AppState, 'appState',
            chlk.models.id.AppId, 'appId',

            [[chlk.models.id.AppId, String, chlk.models.apps.AppState]],
            function $(id, name, state){
                this.setAppId(id);
                this.setAppName(name);
                this.setAppState(state);
            }
        ]);
});
