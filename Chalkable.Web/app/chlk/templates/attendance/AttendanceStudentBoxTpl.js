REQUIRE('chlk.models.attendance.AttendanceStudentBox');
REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AttendanceStudentBoxTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminStudentBox.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceStudentBox)],
        'AttendanceStudentBoxTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Object, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'top',

            [ria.templates.ModelPropertyBind],
            Number, 'currentPage',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevelsIds'
        ])
});