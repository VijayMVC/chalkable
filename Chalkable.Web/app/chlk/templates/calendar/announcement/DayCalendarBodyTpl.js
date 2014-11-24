REQUIRE('chlk.templates.calendar.announcement.BaseCalendarBodyTpl');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.models.calendar.announcement.WeekItem');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";
    /**@class chlk.templates.calendar.announcement.DayCalendarBodyTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/DayCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        'DayCalendarBodyTpl', EXTENDS(chlk.templates.calendar.announcement.BaseCalendarBodyTpl),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items'

    ]);
});