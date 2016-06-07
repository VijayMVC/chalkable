REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.attendance.SeatingChart');
REQUIRE('chlk.models.attendance.ClassList');
REQUIRE('chlk.models.attendance.ClassAttendanceStatsViewData');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassAttendanceSummary*/
    CLASS(
        'ClassAttendanceSummary', EXTENDS(chlk.models.classes.Class),[

            chlk.models.attendance.SeatingChart, 'seatingChart',

            chlk.models.attendance.ClassList, 'classList',

            chlk.models.attendance.ClassAttendanceStatsViewData, 'stats'
    ]);
});