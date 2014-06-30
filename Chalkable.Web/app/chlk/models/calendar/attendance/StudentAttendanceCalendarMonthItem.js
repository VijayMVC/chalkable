REQUIRE('chlk.models.calendar.attendance.AttendanceCalendarMonthItem');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.calendar.attendance', function () {
    "use strict";

    /** @class chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem*/
    CLASS(
        'StudentAttendanceCalendarMonthItem', EXTENDS(chlk.models.calendar.attendance.AttendanceCalendarMonthItem), [

            [ria.serialize.SerializeProperty('morecount')],
            Number, 'moreCount',

            [ria.serialize.SerializeProperty('isexcused')],
            Boolean, 'excused',

            [ria.serialize.SerializeProperty('isabsent')],
            Boolean, 'absent',

            [ria.serialize.SerializeProperty('showgroupeddata')],
            Boolean, 'showGroupedData',

            [ria.serialize.SerializeProperty('personid')],
            chlk.models.id.SchoolPersonId, 'personId'
        ]);
});
