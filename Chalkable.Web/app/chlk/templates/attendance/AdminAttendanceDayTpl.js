REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.attendance.AttendanceDaySummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceDayTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryDay.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceDaySummary)],
        'AdminAttendanceDayTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendancesStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'studentsAbsentWholeDay',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCountAbsentWholeDay',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'absentStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'excusedStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'lateStudents'
        ])
});