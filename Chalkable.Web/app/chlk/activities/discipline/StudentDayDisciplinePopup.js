REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.discipline.StudentDayDisciplineTpl');

NAMESPACE('chlk.activities.discipline', function () {

    /** @class chlk.activities.discipline.StudentDayDisciplinePopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(true)],
        [chlk.activities.lib.isTopLeftPosition(true)],
        [ria.mvc.ActivityGroup('StudentDayDisciplinesPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.StudentDayDisciplineTpl)],
        'StudentDayDisciplinePopup', EXTENDS(chlk.activities.lib.TemplatePopup), [

        ]);
});