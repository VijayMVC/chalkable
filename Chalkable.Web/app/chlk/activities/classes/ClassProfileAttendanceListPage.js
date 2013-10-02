REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassAttendance');
REQUIRE('chlk.templates.attendance.ClassList');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendanceListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#profile')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassProfileAttendanceListTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassProfileAttendanceListTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendanceListPage', EXTENDS(chlk.activities.attendance.ClassListPage),[
    ]);
});