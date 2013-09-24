REQUIRE('chlk.models.calendar.BaseMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');

NAMESPACE('chlk.models.calendar.attendance', function(){
    "use strict";

    /**@class chlk.models.calendar.attendance.StudentAttendanceMonthCalendar*/
    CLASS('StudentAttendanceMonthCalendar', EXTENDS(chlk.models.calendar.BaseMonthCalendar),[

        ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
            , ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem)]],
        function $(date_, minDate_, maxDate_, calendarItems_){
            BASE(date_, minDate_, maxDate_);
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
        }
    ]);
});