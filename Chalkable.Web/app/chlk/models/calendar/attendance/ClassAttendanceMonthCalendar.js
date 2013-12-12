REQUIRE('chlk.models.calendar.BaseMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem');

NAMESPACE('chlk.models.calendar.attendance', function(){
    "use strict";

    /**@class chlk.models.calendar.attendance.ClassAttendanceMonthCalendar*/
    CLASS('ClassAttendanceMonthCalendar', EXTENDS(chlk.models.calendar.BaseMonthCalendar),[

        ArrayOf(chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem), 'calendarItems',

        chlk.models.id.ClassId, 'classId',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
            , ArrayOf(chlk.models.calendar.attendance.ClassAttendanceCalendarMonthItem)
            , chlk.models.id.ClassId, ArrayOf(chlk.models.people.Claim)
        ]],
        function $(date_, minDate_, maxDate_, calendarItems_, classId_, claims_){
            BASE(date_, minDate_, maxDate_);
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
            if(classId_)
                this.setClassId(classId_);
        }
    ]);
});