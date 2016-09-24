REQUIRE('chlk.templates.attendance.ClassList');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassListPeopleTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassListPeople.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassList)],
        'ClassListPeopleTpl', EXTENDS(chlk.templates.attendance.ClassList), []);
});