REQUIRE('chlk.models.developer.DeveloperBalance');
REQUIRE('chlk.models.apps.AppInstallStats');
REQUIRE('chlk.models.apps.AppViewStats');

NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.HomeAnalytics*/
    CLASS(
        'HomeAnalytics', [
            chlk.models.developer.DeveloperBalance, 'devBalance',
            [ria.serialize.SerializeProperty('appInstalls')],
            chlk.models.apps.AppInstallStats, 'appInstallStats',

            [ria.serialize.SerializeProperty('appViews')],
            chlk.models.apps.AppViewStats, 'appViewStats'

        ]);
});
