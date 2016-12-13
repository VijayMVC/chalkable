REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.attendance.StudentAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentProfileAttendanceViewData*/
    CLASS('StudentProfileAttendanceViewData',EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.attendance.StudentAttendanceSummary)), [

        chlk.models.calendar.attendance.StudentAttendanceMonthCalendar, 'attendanceCalendar',

        chlk.models.attendance.StudentAttendanceSummary, function getSummaryInfo(){return this.getUser();},

        [[chlk.models.common.Role,
            chlk.models.attendance.StudentAttendanceSummary
            , chlk.models.calendar.attendance.StudentAttendanceMonthCalendar
            , ArrayOf(chlk.models.people.Claim)
        ]],
        function $(role, summaryInfo, attendanceCalendar, claims_){
            BASE(role, summaryInfo, claims_);
            this.setAttendanceCalendar(attendanceCalendar);
        }
    ]);
});