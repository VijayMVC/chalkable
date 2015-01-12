REQUIRE('chlk.models.calendar.BaseCalendar');


NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.BaseDayWeekCalendar*/
    CLASS(
        'BaseDayWeekCalendar', EXTENDS(chlk.models.calendar.BaseCalendar),  [

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function $(date_, minDate_, maxDate_){
                BASE();
                if(minDate_ && maxDate_)
                    this.setBaseCalendarData(date_ || null, minDate_, maxDate_);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            OVERRIDE, VOID, function setBaseCalendarData(date, minDate, maxDate){
                BASE(date, minDate, maxDate);minDate = null;maxDate=null;

                var date = this.getCurrentDate();
                var dayNumber = date.getDate().getDay(), sunday = date, saturday = date;
                if(dayNumber){
                    sunday = date.add(chlk.models.common.ChlkDateEnum.DAY, -dayNumber);
                }
                if(dayNumber !=6)
                    saturday = sunday.add(chlk.models.common.ChlkDateEnum.DAY, 6);
                var title = sunday.format('MM d - ');
                title = title + (sunday.format('M') == saturday.format('M') ? saturday.format('d') : saturday.format('M d'));
                this.setCurrentTitle(title);
                var prevDate = sunday.add(chlk.models.common.ChlkDateEnum.DAY, -1);
                var nextDate = saturday.add(chlk.models.common.ChlkDateEnum.DAY, 1);
                if(!minDate || prevDate.format('yy-mm-dd') >= minDate.format('yy-mm-dd')){
                    this.setPrevDate(prevDate);
                }
                if(!maxDate || nextDate.format('yy-mm-dd') <= maxDate.format('yy-mm-dd')){
                    this.setNextDate(nextDate);
                }
            }
        ]);
});
