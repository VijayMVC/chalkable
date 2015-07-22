REQUIRE('chlk.models.apps.AppMarketBaseViewData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketDetailsViewData*/
    CLASS(
        'AppMarketDetailsViewData', EXTENDS(chlk.models.apps.AppMarketBaseViewData), [
            chlk.models.apps.AppMarketApplication, 'app',
            String, 'installBtnTitle',
            Boolean, 'installed',

            [[
                chlk.models.apps.AppMarketApplication,
                String,
                ArrayOf(chlk.models.apps.AppCategory),
                ArrayOf(chlk.models.apps.AppGradeLevel),
                Number
            ]],
            function $(app, installBtnTitle, categories, gradelevels, balance, isInstalled) {
                BASE(categories, gradelevels, balance);
                this.setApp(app);
                var webSiteLink = app.getDeveloperInfo().getWebSite() || "";

                if (webSiteLink.indexOf("http") == -1){
                    webSiteLink = "http://" + webSiteLink;
                    app.getDeveloperInfo().setWebSite(webSiteLink);
                }

                this.setInstalled(isInstalled);
                this.setInstallBtnTitle(installBtnTitle);


            }
        ]);


});
