REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceTpl');
REQUIRE('chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendancePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileAttendanceTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl, '', '.calendar-section', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendancePage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});