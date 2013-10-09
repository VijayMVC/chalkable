REQUIRE('chlk.controls.Base');
REQUIRE('chlk.services.BaseService');
REQUIRE('ria.templates.Template');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.models.calendar.ListForWeekCalendar');
REQUIRE('chlk.templates.calendar.ListForWeekCalendarTpl');
REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.controls', function () {
    /** @class chlk.controls.ListForWeekCalendarControl */
    CLASS(
        'ListForWeekCalendarControl', EXTENDS(chlk.controls.Base), [

            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/ListForWeekCalendar.jade')(this);
            },

            VOID, function addCalendar()
            {
                var calendarService = this.getContext().getService(chlk.services.CalendarService);
                calendarService.getListForWeek().then(function(model){
                    var res = new chlk.models.calendar.ListForWeekCalendar(model);
                    var tpl = new chlk.templates.calendar.ListForWeekCalendarTpl();
                    tpl.assign(res);
                    console.info(res,tpl);
                    tpl.renderTo(new ria.dom.Dom('.announcement-week').empty());
                })
            }

        ]);
});
