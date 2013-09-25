REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.templates.calendar.discipline.StudentDisciplineCalendarBodyTpl');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');

NAMESPACE('chlk.templates.calendar.discipline', function () {
    "use strict";

    /** @class chlk.templates.calendar.discipline.StudentDisciplineMonthCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/discipline/StudentDisciplineMonthCalendar.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.discipline.StudentDisciplineMonthCalendar)],
        [chlk.activities.lib.PageClass('calendar')],
        'StudentDisciplineMonthCalendarTpl', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem), 'calendarItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',
            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'minDate',
            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'maxDate'
        ]);
});