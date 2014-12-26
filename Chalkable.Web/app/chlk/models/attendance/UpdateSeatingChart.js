REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.chlk.models.attendance.UpdateSeatingChart*/
    CLASS('UpdateSeatingChart',  [

        chlk.models.common.ChlkDate, 'date',

        Object, 'seatingChartInfo'
    ]);
});
