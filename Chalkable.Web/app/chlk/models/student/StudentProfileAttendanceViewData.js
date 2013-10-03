REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.attendance.StudentAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.student', function(){
   "use strict";

    /**@class chlk.models.student.StudentProfileAttendanceViewData*/
    CLASS('StudentProfileAttendanceViewData',EXTENDS(chlk.models.people.UserProfileViewData), [

        chlk.models.calendar.attendance.StudentAttendanceMonthCalendar, 'attendanceCalendar',
        ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

        chlk.models.attendance.StudentAttendanceSummary, function getSummaryInfo(){return this.getUser();},

        [[chlk.models.common.Role,
            chlk.models.attendance.StudentAttendanceSummary
            , chlk.models.calendar.attendance.StudentAttendanceMonthCalendar
            , ArrayOf(chlk.models.schoolYear.MarkingPeriod)
        ]],
        function $(role, summaryInfo, attendanceCalendar, markingPeriods){
            BASE(role, summaryInfo);
            this.setAttendanceCalendar(attendanceCalendar);
            this.setMarkingPeriods(markingPeriods);
        }
    ]);
});