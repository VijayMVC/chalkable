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
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems'

        ]);
});