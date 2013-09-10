REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketInstallViewData*/
    CLASS(
        'AppMarketInstallViewData', [

            chlk.models.apps.AppMarketApplication, 'app',

            [[chlk.models.apps.AppMarketApplication]],
            function $(app){
                BASE();
                this.setApp(app);
            }
        ]);


});
