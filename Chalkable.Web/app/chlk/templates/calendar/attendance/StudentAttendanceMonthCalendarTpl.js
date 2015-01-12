REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.templates.calendar.attendance.StudentAttendanceCalendarBodyTpl');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');

NAMESPACE('chlk.templates.calendar.attendance', function () {
    "use strict";

    /** @class chlk.templates.calendar.attendance.StudentAttendanceMonthCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/attendance/StudentAttendanceMonthCalendar.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.attendance.StudentAttendanceMonthCalendar)],
        [chlk.activities.lib.PageClass('calendar')],
        'StudentAttendanceMonthCalendarTpl', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',
            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minDate',
            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxDate'
        ]);
});