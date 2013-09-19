REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.calendar.BaseCalendar');


NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Month*/
    CLASS(
        'Month', EXTENDS(chlk.models.calendar.BaseCalendar), [
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename


            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , ArrayOf(chlk.models.calendar.announcement.MonthItem), chlk.models.classes.ClassesForTopBar
            ]],
            function $(date_, minDate_, maxDate_, monthItems_, classes_){
                BASE();
                if(minDate_ && maxDate_)
                    this.setBaseCalendarData(date_ || null, minDate_, maxDate_);
                if(monthItems_)
                    this.setItems(monthItems_);
                if(classes_)
                    this.setTopData(classes_);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            OVERRIDE, VOID, function setBaseCalendarData(date, minDate, maxDate){

                BASE(date, minDate, maxDate);
                date = this.getCurrentDate();
                this.setCurrentTitle(date.format('MM'));

                var year = date.getDate().getFullYear();
                var month = date.getDate().getMonth();
                var day = date.getDate().getDate();
                var prevMonth = month ? month - 1 : 11;
                var prevYear = month ? year : year - 1;
                var prevDate = new Date(prevYear, prevMonth, day);
                var nextMonth = month == 11 ? 0 : month + 1;
                var nextYear = month == 11 ? year + 1 : year;
                var nextDate = new Date(nextYear, nextMonth, day);

                this.setPrevDate(new chlk.models.common.ChlkDate(prevDate));
                this.setNextDate(new chlk.models.common.ChlkDate(nextDate));

                var startDate = minDate.getDate();
                var endDate = maxDate.getDate();
                if(this.getPrevDate() < startDate && startDate.getMonth() != date.getDate().getMonth())
                    this.setPrevDate(minDate);
                if(this.getNextDate() < endDate &&
                    this.getNextDate().getMonth() != date.getDate().getMonth())
                    this.setNextDate(maxDate);
            }
        ]);
});
