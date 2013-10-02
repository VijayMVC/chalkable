REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.attendance.AttendanceNowSummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceNowTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryNow.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceNowSummary)],
        'AdminAttendanceNowTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendanceStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'absentNowStudents',

            [ria.templates.ModelPropertyBind],
            Number, 'absentNowCount',

            [ria.templates.ModelPropertyBind],
            Number, 'absentUsually',

            [ria.templates.ModelPropertyBind],
            Number, 'avgOfAbsentsInYear'
        ])
});