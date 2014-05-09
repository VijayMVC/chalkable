REQUIRE('chlk.models.calendar.attendance.AttendanceCalendarMonthItem');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.calendar.attendance', function () {
    "use strict";

    /** @class chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem*/
    CLASS(
        'ClassAttendanceCalendarMonthItem', EXTENDS(chlk.models.calendar.attendance.AttendanceCalendarMonthItem), [

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId'
    ]);
});
