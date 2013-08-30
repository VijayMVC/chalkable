REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.converters.ClassAttendanceIdToNameConverter');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassAttendance*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassAttendance', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassAttendanceId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPeriodId, 'classPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'type',

            [ria.templates.ModelPropertyBind('type', chlk.converters.ClassAttendanceIdToNameConverter)],
            String, 'typeName',

            [ria.templates.ModelPropertyBind],
            chlk.models.period.Period, 'period',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons'
        ])
});