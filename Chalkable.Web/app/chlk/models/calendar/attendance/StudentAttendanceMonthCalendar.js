REQUIRE('chlk.models.calendar.StudentProfileMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.calendar.attendance', function(){
    "use strict";

    /**@class chlk.models.calendar.attendance.StudentAttendanceMonthCalendar*/
    CLASS('StudentAttendanceMonthCalendar', EXTENDS(chlk.models.calendar.StudentProfileMonthCalendar),[

        ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem),
            chlk.models.id.SchoolPersonId, chlk.models.schoolYear.GradingPeriod]],
        function $(date_, calendarItems_, studentId_, currentGp_){
            if(currentGp_){
                var endDate = currentGp_.getEndDate();
                var startDate = currentGp_.getStartDate();
                var today = new chlk.models.common.ChlkSchoolYearDate();
                calendarItems_.forEach(function(day){
                    var date = day.getDate().getDate();
                    day.setTodayClassName((today.format('mm-dd-yy') == day.getDate().format('mm-dd-yy')) ? 'today' : '');
                    day.setClassName((day.isCurrentMonth() && date >= startDate.getDate() &&
                        date <= endDate.getDate()) ? '' : 'not-current-month');
                });
            }

            BASE(date_, startDate, endDate, studentId_);

            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
        }
    ]);
});