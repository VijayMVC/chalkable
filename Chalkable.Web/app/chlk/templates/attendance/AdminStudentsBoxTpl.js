REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.AdminStudentsBox');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminStudentsBoxTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminStudentsBox.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AdminStudentsBox)],
        'AdminStudentsBoxTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            ArrayOf(chlk.models.people.User), 'students'
        ])
});