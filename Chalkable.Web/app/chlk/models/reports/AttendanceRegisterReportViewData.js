REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.AttendanceRegisterReportViewData*/
    CLASS('AttendanceRegisterReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        ArrayOf(chlk.models.attendance.AttendanceReason), 'attendanceReasons',

        ArrayOf(chlk.models.common.NameId), 'attendanceMonths',

        [[ArrayOf(chlk.models.attendance.AttendanceReason), ArrayOf(chlk.models.common.NameId), chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, Boolean, Boolean]],
        function $(attendanceReasons_, attendanceMonths_, classId_, gradingPeriodId_, ableDownload_, isAbleToReadSSNumber_){
            BASE(classId_, gradingPeriodId_, null, null, null, ableDownload_, isAbleToReadSSNumber_);
            if(attendanceReasons_)
                this.setAttendanceReasons(attendanceReasons_);
            if(attendanceMonths_)
                this.setAttendanceMonths(attendanceMonths_);
        }
    ]);
});