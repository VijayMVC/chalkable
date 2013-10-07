REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');


NAMESPACE('chlk.templates.calendar.attendance', function (){
    "use strict";

    /**@class chlk.templates.calendar.attendance.StudentAttendanceCalendarBodyTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/attendance/StudentAttendanceMonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.attendance.StudentAttendanceMonthCalendar)],
        'StudentAttendanceCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

            [[Number]],
            String, function getColorByAttendanceType(attendanceType){
                var attTypeEnums = chlk.models.attendance.AttendanceTypeEnum;
                switch (attendanceType){
                    case attTypeEnums.PRESENT.valueOf() : return 'green';
                    case attTypeEnums.ABSENT.valueOf() : return 'red';
                    case attTypeEnums.EXCUSED.valueOf() : return 'blue';
                    case attTypeEnums.LATE.valueOf() : return 'darkOrange';
                    default : return 'black';
                }
            }
        ]);
});