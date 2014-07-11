REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceMonthCalendar');


NAMESPACE('chlk.templates.calendar.attendance', function (){
    "use strict";

    /**@class chlk.templates.calendar.attendance.ClassAttendanceCalendarBodyTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/attendance/ClassAttendanceMonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.attendance.ClassAttendanceMonthCalendar)],
        'ClassAttendanceCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem), 'calendarItems'

        ]);
});