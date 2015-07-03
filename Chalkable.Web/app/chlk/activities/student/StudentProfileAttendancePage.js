REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileAttendanceTpl');
REQUIRE('chlk.templates.calendar.attendance.StudentAttendanceMonthCalendarTpl');
REQUIRE('chlk.templates.calendar.attendance.StudentAttendanceCalendarBodyTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileAttendancePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.PartialUpdateRule(chlk.templates.calendar.attendance.StudentAttendanceMonthCalendarTpl, '', '.calendar-panel', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfileAttendanceTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileAttendanceTpl)],
        'StudentProfileAttendancePage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});