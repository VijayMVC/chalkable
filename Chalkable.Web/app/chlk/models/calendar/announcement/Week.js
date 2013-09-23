REQUIRE('chlk.models.calendar.announcement.WeekItem');
REQUIRE('chlk.models.calendar.BaseCalendar');
REQUIRE('chlk.models.grading.GradeLevelsForTopBar');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Week*/
    CLASS(
        'Week', EXTENDS(chlk.models.calendar.BaseCalendar), [
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items',

            chlk.models.classes.ClassesForTopBar, 'topData',  //todo: rename

            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar',

            Boolean, 'admin',

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , ArrayOf(chlk.models.calendar.announcement.WeekItem)
                , chlk.models.classes.ClassesForTopBar
                , chlk.models.grading.GradeLevelsForTopBar
            ]],
            function $(date_, minDate_, maxDate_, weekItems_, classes_, gradeLevels_){
                BASE();
                if(minDate_ && maxDate_)
                    this.setBaseCalendarData(date_ || null, minDate_, maxDate_);
                if(weekItems_)
                    this.setItems(weekItems_);
                if(classes_)
                    this.setTopData(classes_);
                if(gradeLevels_)
                    this.setGradeLevelsForToolBar(gradeLevels_);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            OVERRIDE, VOID, function setBaseCalendarData(date, minDate, maxDate){

                BASE(date, minDate, maxDate);
                //todo think how to write better this  logic
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
