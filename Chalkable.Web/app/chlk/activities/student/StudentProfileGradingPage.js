REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl');
REQUIRE('chlk.templates.student.StudentProfileGradingTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        //[ria.mvc.PartialUpdateRule(chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl, '', '#discipline-calendar-info', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileGradingTpl)],
        'StudentProfileGradingPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.new-grading-box.item-type .item-type-title')],
            [[ria.dom.Dom, ria.dom.Event]],
            function showItems(node, event){
                node.parent('.item-type').find('.item-type-details').toggleClass('x-hidden');
            }
        ]);
});