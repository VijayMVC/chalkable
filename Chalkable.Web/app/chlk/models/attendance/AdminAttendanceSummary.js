REQUIRE('chlk.models.common.PageWithGrades');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.attendance.AttendanceNowSummary');
REQUIRE('chlk.models.attendance.AttendanceDaySummary');
REQUIRE('chlk.models.attendance.AttendanceMpSummary');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AdminAttendanceSummary*/
    CLASS(
        'AdminAttendanceSummary', EXTENDS(chlk.models.common.PageWithGrades), [
            [ria.serialize.SerializeProperty('nowattendancedata')],
            chlk.models.attendance.AttendanceNowSummary, 'nowAttendanceData',

            [ria.serialize.SerializeProperty('attendancebydaydata')],
            chlk.models.attendance.AttendanceDaySummary, 'attendanceByDayData',

            [ria.serialize.SerializeProperty('attendancebympdata')],
            chlk.models.attendance.AttendanceMpSummary, 'attendanceByMpData',

            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

            Boolean, 'renderNow',

            Number, 'currentPage',

            Boolean, 'renderDay',

            Boolean, 'renderMp',

            String, 'gradeLevelsIds',

            chlk.models.common.ChlkDate, 'nowDateTime',

            chlk.models.id.MarkingPeriodId, 'fromMarkingPeriodId',

            chlk.models.id.MarkingPeriodId, 'toMarkingPeriodId',

            chlk.models.common.ChlkDate, 'startDate',

            chlk.models.common.ChlkDate, 'endDate',
        ]);
});
