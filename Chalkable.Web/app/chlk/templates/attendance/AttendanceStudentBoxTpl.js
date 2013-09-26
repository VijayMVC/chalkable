REQUIRE('chlk.models.attendance.AttendanceStudentBox');
REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AttendanceStudentBoxTpl.js*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AttendanceStudentBox.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceStudentBox)],
        'AttendanceStudentInfoTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date'
        ])
});