REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.attendance.StudentDayAttendances');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.StudentReasonsComboTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentReasonsCombo.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.StudentDayAttendances)],
        'StudentReasonsComboTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            ArrayOf(chlk.models.attendance.AttendanceReason), 'currentReasons',

            chlk.models.id.AttendanceReasonId, 'currentId'
        ])
});