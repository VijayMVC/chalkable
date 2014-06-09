REQUIRE('chlk.templates.people.UserTpl');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminStudentSearchTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminStudentSearch.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'AdminStudentSearchTpl', EXTENDS(chlk.templates.people.UserTpl), [])
});