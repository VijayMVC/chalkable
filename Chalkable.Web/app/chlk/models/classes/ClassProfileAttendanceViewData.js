REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.ClassAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.ClassAttendanceMonthCalendar');

NAMESPACE('chlk.models.classes', function(){
    "use strict";
    /**@class chlk.models.classes.ClassProfileAttendanceViewData*/
    CLASS('ClassProfileAttendanceViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData), [

        chlk.models.calendar.attendance.ClassAttendanceMonthCalendar, 'monthCalendar',

        [[chlk.models.common.Role, chlk.models.classes.ClassAttendanceSummary
            , chlk.models.calendar.attendance.ClassAttendanceMonthCalendar
            , ArrayOf(chlk.models.people.Claim)]],
        function $(role_, classAttSummary_, monthCalendar_, claims_){
            BASE(role_, classAttSummary_, claims_);
            if(monthCalendar_)
                this.setMonthCalendar(monthCalendar_);
        }
    ]);
});