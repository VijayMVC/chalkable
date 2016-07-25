REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.discipline.StudentDayDisciplines');

NAMESPACE('chlk.templates.discipline', function () {

    /** @class chlk.templates.discipline.StudentDayDisciplineTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/StudentDayDiscipline.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.StudentDayDisciplines)],
        'StudentDayDisciplineTpl', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem, 'item',

            [ria.templates.ModelPropertyBind],
            Boolean, 'canSetDiscipline',

            [ria.templates.ModelPropertyBind],
            String, 'controller',

            [ria.templates.ModelPropertyBind],
            String, 'action',

            [ria.templates.ModelPropertyBind],
            String, 'params'
        ])
});