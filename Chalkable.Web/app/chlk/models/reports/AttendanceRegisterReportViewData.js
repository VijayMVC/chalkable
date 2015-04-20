REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.AttendanceRegisterReportViewData*/
    CLASS('AttendanceRegisterReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        ArrayOf(chlk.models.attendance.AttendanceReason), 'attendanceReasons',

        [[ArrayOf(chlk.models.attendance.AttendanceReason), chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(attendanceReasons_, classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
            if(attendanceReasons_)
                this.setAttendanceReasons(attendanceReasons_);
        }
    ]);
});