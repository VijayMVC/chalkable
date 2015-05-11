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
        function $(date_, minDate_, maxDate_, calendarItems_, classId_){
            BASE(date_, minDate_, maxDate_);
            if(minDate_ && maxDate_){
                var endDate = maxDate_;
                var startDate = minDate_;
                var today = new chlk.models.common.ChlkSchoolYearDate();
                calendarItems_.forEach(function(day){
                    var date = day.getDate().getDate();
                    day.setTodayClassName((today.format('mm-dd-yy') == day.getDate().format('mm-dd-yy')) ? 'today' : '');
                    day.setClassName((day.isCurrentMonth() && date >= startDate.getDate() &&
                        date <= endDate.getDate()) ? '' : 'not-current-month');
                });
            }
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
            if(classId_)
                this.setClassId(classId_);
        }
    ]);
});