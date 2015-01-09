REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.announcement.Week');


NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.BaseCalendarBodyTpl*/
    CLASS(
        //[ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekCalendarBody.jade')],
        //[ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        'BaseCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[
            Boolean, 'mainCalendar'
        ]);
});