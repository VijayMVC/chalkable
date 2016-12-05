REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.lunchCount.LunchCountGrid');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassProfileLunchSummary*/
    CLASS(
        'ClassProfileLunchSummary', EXTENDS(chlk.models.classes.Class), [

            chlk.models.lunchCount.LunchCountGrid, 'lunchCountInfo',

            Boolean, 'ableSubmit',

            ArrayOf(chlk.models.common.ChlkDate), 'scheduledDays'
        ]);
});
