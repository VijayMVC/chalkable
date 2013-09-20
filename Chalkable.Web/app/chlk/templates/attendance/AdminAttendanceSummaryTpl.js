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