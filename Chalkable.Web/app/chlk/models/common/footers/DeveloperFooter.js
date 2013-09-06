REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.common.footers', function () {
    "use strict";

    /** @class chlk.models.common.footers.DeveloperFooter*/
    CLASS(
        'DeveloperFooter', [
            chlk.models.id.AppId, 'currentAppId',
            ArrayOf(chlk.models.apps.Application), 'developerApps',

            [[chlk.models.id.AppId,  ArrayOf(chlk.models.apps.Application)]],
            function $(currentAppId, devApps){
                BASE();
                this.setCurrentAppId(currentAppId);
                this.setDeveloperApps(devApps);
            }
        ]);
});
