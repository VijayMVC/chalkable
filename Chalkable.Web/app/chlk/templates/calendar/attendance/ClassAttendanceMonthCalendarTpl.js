REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.templates.calendar.attendance.ClassAttendanceCalendarBodyTpl');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceMonthCalendar');

NAMESPACE('chlk.templates.calendar.attendance', function () {
    "use strict";

    /** @class chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/attendance/ClassAttendanceMonthCalendar.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.attendance.ClassAttendanceMonthCalendar)],
        [chlk.activities.lib.PageClass('calendar')],
        'ClassAttendanceMonthCalendarTpl', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem), 'calendarItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId'
    ]);
});