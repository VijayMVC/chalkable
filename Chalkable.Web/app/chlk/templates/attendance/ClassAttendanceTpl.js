REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassAttendanceTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassAttendanceId, 'id',

//            [ria.templates.ModelPropertyBind],
//            chlk.models.id.ClassPersonId, 'classPersonId',
//
//            [ria.templates.ModelPropertyBind],
//            chlk.models.id.ClassPeriodId, 'classPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'type',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind('type', chlk.converters.attendance.AttendanceTypeToNameConverter)],
            String, 'typeName',
//
//            [ria.templates.ModelPropertyBind],
//            chlk.models.period.Period, 'period',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            Boolean, 'needPresent',

            String, function getSubmitFormActionName(){
                return 'setAttendanceProfile';
            },
        ])
});