NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppViewStatsItem*/
    CLASS(
        'AppViewStatsItem', [
            Number, 'viewsCount',
            String, 'summary',
            String, 'role'
        ]);

    /** @class chlk.models.apps.AppViewStats*/
    CLASS(
        'AppViewStats', [
            Number, 'totalCount',
            ArrayOf(chlk.models.apps.AppViewStatsItem), 'stats'
        ])

});
