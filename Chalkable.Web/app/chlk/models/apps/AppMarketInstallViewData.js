REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppInstallGroup');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketInstallViewData*/
    CLASS(
        'AppMarketInstallViewData',EXTENDS(chlk.models.apps.AppMarketDetailsViewData), [
            Boolean, 'alreadyInstalled',

            [[chlk.models.apps.AppMarketApplication]],
            function $(app){
                BASE(app, "", [], [], 0, app.isAlreadyInstalled());
                this.setAlreadyInstalled(app.isAlreadyInstalled());
            }
        ]);


});
