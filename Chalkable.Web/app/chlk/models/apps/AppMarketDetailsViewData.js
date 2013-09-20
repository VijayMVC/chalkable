REQUIRE('chlk.models.apps.AppMarketApplication');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketDetailsViewData*/
    CLASS(
        'AppMarketDetailsViewData', [
            chlk.models.apps.AppMarketApplication, 'app',
            String, 'installBtnTitle',

            [[chlk.models.apps.AppMarketApplication, String]],
            function $(app, installBtnTitle){
                BASE();
                this.setApp(app);
                this.setInstallBtnTitle(installBtnTitle);
            }
        ]);


});
