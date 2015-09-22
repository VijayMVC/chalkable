NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppInstallStatsItem*/
    CLASS(
        'AppInstallStatsItem', [
            Number, 'installCount',
            String, 'summary'
        ]);

    /** @class chlk.models.apps.AppInstallStats*/
    CLASS(
        'AppInstallStats', [
            Number, 'totalCount',
            ArrayOf(chlk.models.apps.AppInstallStatsItem), 'stats'
        ])

});
