REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.AttendanceProfileReportViewData*/
    CLASS('AttendanceProfileReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'attendanceReasons',

        [[ArrayOf(chlk.models.schoolYear.MarkingPeriod), ArrayOf(chlk.models.attendance.AttendanceReason), ArrayOf(chlk.models.people.ShortUserInfo),
            chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean]],
        function $(markingPeriods_, attendanceReasons_, students_, classId_, gradingPeriodId_, startDate_, endDate_, ableDownload_, isAbleToReadSSNumber_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_, students_, ableDownload_, isAbleToReadSSNumber_);
            if(markingPeriods_)
                this.setMarkingPeriods(markingPeriods_);
            if(attendanceReasons_)
                this.setAttendanceReasons(attendanceReasons_);
        }
    ]);
});