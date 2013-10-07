REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');

NAMESPACE('chlk.templates.calendar.discipline', function (){
    "use strict";

    /**@class chlk.templates.calendar.discipline.StudentDisciplineCalendarBodyTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/discipline/StudentDisciplineMonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.discipline.StudentDisciplineMonthCalendar)],
        'StudentDisciplineCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem), 'calendarItems'
        ]);
});