REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppGeneralInfoViewData*/
    CLASS(
        'AppGeneralInfoViewData', [
            chlk.models.apps.Application, 'app',
            chlk.models.apps.Application, 'liveApp',
            Boolean, 'isLiveApp',
            String, 'appThumbnail',
            [[chlk.models.apps.Application, chlk.models.apps.Application, String]],
            function $(app, liveApp, appThumbnail){
                BASE();
                this.setApp(app);
                this.setLiveApp(liveApp);
                this.setAppThumbnail(appThumbnail);
                this.setIsLiveApp(app.getId() != liveApp.getId());
            }
        ]);
});
