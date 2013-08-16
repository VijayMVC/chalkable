REQUIRE('chlk.models.calendar.announcement.Day');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Month*/
    CLASS(
        'Month', [
            ArrayOf(chlk.models.calendar.announcement.Day), 'days',
            [[ArrayOf(chlk.models.calendar.announcement.Day)]],
            function $(days){
                this.setDays(days);
            }
        ]);
});
