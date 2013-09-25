REQUIRE('chlk.models.calendar.StudentProfileMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');

NAMESPACE('chlk.models.calendar.attendance', function(){
    "use strict";

    /**@class chlk.models.calendar.attendance.StudentAttendanceMonthCalendar*/
    CLASS('StudentAttendanceMonthCalendar', EXTENDS(chlk.models.calendar.StudentProfileMonthCalendar),[

        ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
            , ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), chlk.models.id.SchoolPersonId]],
        function $(date_, minDate_, maxDate_, calendarItems_, studentId_){
            BASE(date_, minDate_, maxDate_, studentId_);
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
        }
    ]);
});