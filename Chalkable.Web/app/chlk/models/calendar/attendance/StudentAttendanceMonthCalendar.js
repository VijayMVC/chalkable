REQUIRE('chlk.models.calendar.StudentProfileMonthCalendar');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.calendar.attendance', function(){
    "use strict";

    /**@class chlk.models.calendar.attendance.StudentAttendanceMonthCalendar*/
    CLASS('StudentAttendanceMonthCalendar', EXTENDS(chlk.models.calendar.StudentProfileMonthCalendar),[

        ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, ArrayOf(chlk.models.calendar.attendance.StudentAttendanceCalendarMonthItem),
            chlk.models.id.SchoolPersonId, chlk.models.schoolYear.MarkingPeriod]],
        function $(date_, calendarItems_, studentId_, currentMp_){
            if(currentMp_){
                var endDate = currentMp_.getEndDate();
                var startDate = currentMp_.getStartDate();
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