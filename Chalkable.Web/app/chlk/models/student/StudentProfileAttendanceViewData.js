REQUIRE('chlk.models.attendance.StudentAttendanceSummary');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.student', function(){
   "use strict";

    /**@class chlk.models.student.StudentProfileAttendanceViewData*/
    CLASS('StudentProfileAttendanceViewData',[

        chlk.models.attendance.StudentAttendanceSummary, 'summaryInfo',
        chlk.models.calendar.attendance.StudentAttendanceMonthCalendar, 'attendanceCalendar',
        ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

        [[chlk.models.attendance.StudentAttendanceSummary
            , chlk.models.calendar.attendance.StudentAttendanceMonthCalendar
            , ArrayOf(chlk.models.schoolYear.MarkingPeriod)
        ]],
        function $(summaryInfo, attendanceCalendar, markingPeriods){
            BASE();
            this.setSummaryInfo(summaryInfo);
            this.setAttendanceCalendar(attendanceCalendar);
            this.setMarkingPeriods(markingPeriods);
        }
    ]);
});