REQUIRE('chlk.templates.common.PageWithGrades');
REQUIRE('chlk.models.attendance.AdminAttendanceSummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AdminAttendanceSummary)],
        'AdminAttendanceSummaryTpl', EXTENDS(chlk.templates.common.PageWithGrades), [
            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceNowSummary, 'nowAttendanceData',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceDaySummary, 'attendanceByDayData',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceMpSummary, 'attendanceByMpData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderNow',

            [ria.templates.ModelPropertyBind],
            Number, 'currentPage',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderDay',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderMp',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevelsIds',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'nowDateTime',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.MarkingPeriodId, 'fromMarkingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.MarkingPeriodId, 'toMarkingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [[ArrayOf(chlk.models.people.User), Boolean]],
            function getPreparedStudents(students, needPlus_){
                var lineSize = 6;
                if(needPlus_)
                    students.unshift(new chlk.models.people.User);
                var len = students.length;
                var count = len % lineSize;
                if(!len || count)
                    for(var i = count; i < lineSize; i++)
                        students.push(new chlk.models.people.User);
            }
        ])
});