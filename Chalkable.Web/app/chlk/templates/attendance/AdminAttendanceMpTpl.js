REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.AttendanceMpSummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceMpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryMp.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceMpSummary)],
        'AdminAttendanceMpTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendanceStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'absentAndLateStudents',

            [ria.templates.ModelPropertyBind],
            Number, 'absentStudentsCountAvg'
        ])
});