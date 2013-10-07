REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.calendar.announcement.Month');
REQUIRE('chlk.models.calendar.announcement.MonthItem');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";
    /**@class chlk.templates.calendar.announcement.MonthCalendarBodyTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthCalendarBody.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Month)],
        'MonthCalendarBodyTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items'

    ]);
});