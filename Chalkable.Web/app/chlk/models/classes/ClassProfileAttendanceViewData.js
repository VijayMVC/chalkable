REQUIRE('chlk.models.classes.ClassAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceMonthCalendar');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassProfileAttendanceViewData*/
    CLASS('ClassProfileAttendanceViewData', [

        chlk.models.classes.ClassAttendanceSummary, 'classAttendanceSummary',

        chlk.models.calendar.attendance.ClassAttendanceMonthCalendar, 'monthCalendar',

        [[chlk.models.classes.ClassAttendanceSummary, chlk.models.calendar.attendance.ClassAttendanceMonthCalendar]],
        function $(classAttSummary_, monthCalendar_){
            BASE();
            if(classAttSummary_)
                this.setClassAttendanceSummary(classAttSummary_);
            if(monthCalendar_)
                this.setMonthCalendar(monthCalendar_);
        }
    ]);
});