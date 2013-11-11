REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassAttendanceTpl');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceListTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendanceListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileAttendanceListTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfileAttendanceListTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendanceListPage', EXTENDS(chlk.activities.attendance.ClassListPage),[
    ]);
});