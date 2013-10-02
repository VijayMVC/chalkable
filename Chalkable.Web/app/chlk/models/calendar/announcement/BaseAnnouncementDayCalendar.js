REQUIRE('chlk.models.calendar.BaseCalendar');


NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.BaseAnnouncementDayCalendar*/
    CLASS(
        'BaseAnnouncementDayCalendar', EXTENDS(chlk.models.calendar.BaseDayWeekCalendar),  [

            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items',

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , chlk.models.common.ChlkDate, ArrayOf(chlk.models.calendar.announcement.DayItem)]],
            function $(date_, minDate_, maxDate_, dayItems_){
                BASE(date_, minDate_, maxDate_);
                if(dayItems_)
                    this.initItems_(dayItems_);
            },


            VOID, function initItems_(dayItems){
                var sunday = this.getCurrentDate()
                    , dayNumber = sunday.getDate().getDay()
                    , today = new chlk.models.common.ChlkDate(new Date());
                if(dayNumber)
                    sunday = sunday.add(chlk.models.common.ChlkDateEnum.DAY, -dayNumber);
                var max = 0, index = 0, len, item;
                var days = dayItems;
                if(days.length == 6){
                    sunday = new chlk.models.calendar.announcement.DayItem();
                    var sunDate = days[0].getDate().add(chlk.models.common.ChlkDateEnum.DAY, -1);
                    sunday.setDate(sunDate);
                    sunday.setDay(sunDate.getDate().getDate());
                    sunday.setCalendarDayItems([]);
                    days.unshift(sunday);
                }
                days.forEach(function(day, i){
                    day.setTodayClassName(today.isSameDay(day.getDate()) ? 'today' : '');
                    if(!day.getCalendarDayItems())
                        day.setCalendarDayItems([]);
                    len = day.getCalendarDayItems().length;
                    if(max < len){
                        max = len;
                        index = i;
                    }
                });
                days.forEach(function(day){
                    len = day.getCalendarDayItems().length;
                    if(max > len){
                        for(var i = len; i < max; i++){
                            item = new chlk.models.calendar.announcement.CalendarDayItem();
                            item.setPeriod(days[index].getCalendarDayItems()[i].getPeriod());
                            item.setAnnouncementClassPeriods([]);
                            day.getCalendarDayItems().push(item);
                        }
                    }
                });
                this.setItems(days);
            }
        ]);
});
