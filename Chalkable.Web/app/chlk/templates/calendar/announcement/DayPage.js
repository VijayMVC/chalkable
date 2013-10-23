REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.models.calendar.announcement.Day');

NAMESPACE('chlk.templates.calendar.announcement', function () {
    "use strict";

    /** @class chlk.templates.calendar.announcement.DayPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/dayPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Day)],
        [chlk.activities.lib.PageClass('calendar')],
        'DayPage', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            Boolean, 'notMainCalendar',

            String, 'controllerName',

            String, 'actionName',

            Array, 'params'
        ]);
});