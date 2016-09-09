NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppViewStatsItem*/
    CLASS(
        'AppViewStatsItem', [
            Number, 'viewsCount',
            String, 'summary',
            String, 'type'
        ]);

    /** @class chlk.models.apps.AppViewStats*/
    CLASS(
        'AppViewStats', [
            ArrayOf(chlk.models.apps.AppViewStatsItem), 'stats'
        ])

});
