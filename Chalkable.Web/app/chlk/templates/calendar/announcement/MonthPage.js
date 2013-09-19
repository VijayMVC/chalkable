REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.models.calendar.announcement.Month');

NAMESPACE('chlk.templates.calendar.announcement', function () {
    "use strict";

    /** @class chlk.templates.calendar.announcement.MonthPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Month)],
        [chlk.activities.lib.PageClass('calendar')],
        'MonthPage', EXTENDS(chlk.templates.calendar.BaseCalendarTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData'
        ]);
});