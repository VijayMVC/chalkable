REQUIRE('chlk.templates.attendance.ClassAttendanceTpl');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.attendance.ClassProfileAttendanceListItemTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassProfileAttendanceListItemTpl', EXTENDS(chlk.templates.attendance.ClassAttendanceTpl), [
            OVERRIDE, String, function getSubmitFormActionName(){
                return 'setAttendanceProfile';
            }
        ])
});