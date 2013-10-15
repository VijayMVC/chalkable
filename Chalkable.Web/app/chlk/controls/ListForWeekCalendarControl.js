REQUIRE('chlk.controls.Base');
REQUIRE('chlk.models.common.ChlkDate');
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

            [[chlk.models.common.ChlkDate]],
            VOID, function addCalendar(date_)
            {
                var calendarService = this.getContext().getService(chlk.services.CalendarService);
                calendarService.getListForWeek(date_).then(function(model){
                    var res = new chlk.models.calendar.ListForWeekCalendar(model);
                    var tpl = new chlk.templates.calendar.ListForWeekCalendarTpl();
                    tpl.assign(res);
                    tpl.renderTo(new ria.dom.Dom('.announcement-week').empty());
                })
            },

            [ria.mvc.DomEventBind('click', '.list-for-week-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function clickPrevNextBtn(node, event) {
                var date = new chlk.models.common.ChlkDate(getDate(node.getData('date')));
                new ria.dom.Dom('.announcement-week-loader').addClass('loading');
                this.addCalendar(date);
                return false;
            }
        ]);
});
