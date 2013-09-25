REQUIRE('chlk.models.calendar.BaseMonthCalendar');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem');

NAMESPACE('chlk.models.calendar.discipline', function(){
    "use strict";

    /**@class chlk.models.calendar.discipline.StudentDisciplineMonthCalendar*/
    CLASS('StudentDisciplineMonthCalendar', EXTENDS(chlk.models.calendar.BaseMonthCalendar),[

        ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
            , ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem)]],
        function $(date_, minDate_, maxDate_, calendarItems_){
            BASE(date_, minDate_, maxDate_);
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
        }
    ]);
});