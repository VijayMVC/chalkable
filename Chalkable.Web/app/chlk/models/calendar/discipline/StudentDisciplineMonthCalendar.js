REQUIRE('chlk.models.calendar.StudentProfileMonthCalendar');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem');

NAMESPACE('chlk.models.calendar.discipline', function(){
    "use strict";

    /**@class chlk.models.calendar.discipline.StudentDisciplineMonthCalendar*/
    CLASS('StudentDisciplineMonthCalendar', EXTENDS(chlk.models.calendar.StudentProfileMonthCalendar),[

        ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem), 'calendarItems',

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
            , ArrayOf(chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem)
            , chlk.models.id.SchoolPersonId]],
        function $(date_, minDate_, maxDate_, calendarItems_, studentId_){
            BASE(date_, minDate_, maxDate_, studentId_);
            if(calendarItems_)
                this.setCalendarItems(calendarItems_);
        }
    ]);
});