REQUIRE('chlk.controls.Base');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.services.BaseService');
REQUIRE('ria.templates.Template');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.models.calendar.ListForWeekCalendar');
REQUIRE('chlk.templates.calendar.ListForWeekCalendarTpl');
REQUIRE('chlk.models.announcement.AnnouncementType');

NAMESPACE('chlk.controls', function () {
    chlk.controls.updateWeekCalendar = function () {
        var node = new ria.dom.Dom('.announcement-week');
        chlk.controls.ListForWeekCalendarControl.prototype.addCalendar.call(node.getData('control'), node.getData('date'));
    };

    /** @class chlk.controls.ListForWeekCalendarControl */
    CLASS(
        'ListForWeekCalendarControl', EXTENDS(chlk.controls.Base), [

            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/ListForWeekCalendar.jade')(this);
            },

            [[chlk.models.common.ChlkDate, Boolean]],
            VOID, function addCalendar(date_, needDatePickerUpdate_)
            {
                if(this.getContext){
                    var calendarService = this.getContext().getService(chlk.services.CalendarService);
                    calendarService.getListForWeek(date_).then(function(model){
                        var res = new chlk.models.calendar.ListForWeekCalendar(model);
                        var tpl = new chlk.templates.calendar.ListForWeekCalendarTpl();
                        tpl.assign(res);
                        var dom = new ria.dom.Dom('.announcement-week');
                        dom.setData('control', this);
                        dom.setData('date', date_ || null);
                        //var oldPicker = dom.find('#list-for-week-date');
                        //var scope = oldPicker.getData('control');
                        tpl.renderTo(dom.empty());
                        //if(needDatePickerUpdate_){
                        //    chlk.controls.updateDatePicker(scope, dom.find('#list-for-week-date'), date_);
                        //}
                    }, this)
                }
            },

            [ria.mvc.DomEventBind('click', '.list-for-week-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clickPrevNextBtn(node, event) {
                var date = new chlk.models.common.ChlkSchoolYearDate.$createServerTime(node.getData('date'));
                new ria.dom.Dom('.announcement-week-loader').addClass('loading');
                this.addCalendar(date, true);
                return false;
            },

            [ria.mvc.DomEventBind('change', '#list-for-week-date')],
            [[ria.dom.Dom, ria.dom.Event]],
            function dateChange(node, event) {
                var date = new chlk.models.common.ChlkSchoolYearDate.$createServerTime(node.getValue());
                new ria.dom.Dom('.announcement-week-loader').addClass('loading');
                this.addCalendar(date, true);
            }
        ]);
});
