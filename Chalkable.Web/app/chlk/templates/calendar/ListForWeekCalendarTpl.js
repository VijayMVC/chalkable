REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.ListForWeekCalendar');

NAMESPACE('chlk.templates.calendar', function () {
    "use strict";
    /** @class chlk.templates.calendar.ListForWeekCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/ListForWeekCalendar.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.ListForWeekCalendar)],
        'ListForWeekCalendarTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.ListForWeekCalendarItem), 'items'
        ]);
});