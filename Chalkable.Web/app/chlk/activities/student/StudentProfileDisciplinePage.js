REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');
REQUIRE('chlk.templates.student.StudentProfileDisciplineTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileDisciplinePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.PartialUpdateRule(chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl, '', '.calendar-panel', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfileDisciplineTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileDisciplineTpl)],
        'StudentProfileDisciplinePage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});