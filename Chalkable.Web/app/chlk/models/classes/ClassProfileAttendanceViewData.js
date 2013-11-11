REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceMonthCalendar');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileAttendanceViewData*/
    CLASS('ClassProfileAttendanceViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        chlk.models.calendar.attendance.ClassAttendanceMonthCalendar, 'monthCalendar',

        [[chlk.models.common.Role, chlk.models.classes.ClassAttendanceSummary
            , chlk.models.calendar.attendance.ClassAttendanceMonthCalendar]],
        function $(role_, classAttSummary_, monthCalendar_){
            BASE(role_, classAttSummary_);
            if(monthCalendar_)
                this.setMonthCalendar(monthCalendar_);
        }
    ]);
});