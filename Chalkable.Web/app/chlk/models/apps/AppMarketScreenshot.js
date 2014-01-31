REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketScreenshot*/
    CLASS(
        'AppMarketScreenshot', [
            chlk.models.apps.AppPicture, 'picture',
            chlk.models.id.AppId, 'appId',

            [[chlk.models.apps.AppPicture, chlk.models.id.AppId]],
            function $(screenshot, appId){
                BASE();
                this.setPicture(screenshot);
                this.setAppId(appId);
            }

        ]);


});
