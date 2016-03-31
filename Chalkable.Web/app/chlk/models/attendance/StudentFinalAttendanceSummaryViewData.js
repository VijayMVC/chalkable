REQUIRE('chlk.models.attendance.TotalAttendanceViewData');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.StudentFinalAttendanceSummaryViewData*/
    CLASS(
        'StudentFinalAttendanceSummaryViewData', [
            [ria.serialize.SerializeProperty('totalstudentattendance')],
            chlk.models.attendance.TotalAttendanceViewData, 'totalStudentAttendance',

            [ria.serialize.SerializeProperty('totalclassattendance')],
            chlk.models.attendance.TotalAttendanceViewData, 'totalClassAttendance'
        ]);
});
