REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');


NAMESPACE('chlk.templates.calendar.attendance', function (){
    "use strict";

    /**@class chlk.templates.calendar.attendance.StudentAttendanceCalendarBodyTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/attendance/StudentAttendanceMonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.attendance.StudentAttendanceMonthCalendar)],
        'StudentAttendanceCalendarBodyTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems'

        ]);
});