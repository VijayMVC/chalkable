REQUIRE('chlk.models.apps.AppMarketBaseViewData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketDetailsViewData*/
    CLASS(
        'AppMarketDetailsViewData', EXTENDS(chlk.models.apps.AppMarketBaseViewData), [
            chlk.models.apps.AppMarketApplication, 'app',
            String, 'installBtnTitle',

            [[
                chlk.models.apps.AppMarketApplication,
                String,
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                Number
            ]],
            function $(app, installBtnTitle, categories, gradelevels, balance){
                BASE(categories, gradelevels, balance);
                this.setApp(app);
                this.setInstallBtnTitle(installBtnTitle);
            }
        ]);


});
