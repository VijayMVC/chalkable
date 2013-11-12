REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.calendar.announcement.DayItem');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";
    /**@class chlk.templates.calendar.announcement.DayCalendarBodyTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/DayCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Day)],
        'DayCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items'

    ]);
});