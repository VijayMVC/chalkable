REQUIRE('chlk.models.calendar.BaseCalendar');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.calendar.announcement.AdminDayCalendarItem');
REQUIRE('chlk.models.grading.GradeLevelsForTopBar');

NAMESPACE('chlk.models.calendar.announcement', function (){
    "use strict";

    /**@class chlk.models.calendar.announcement.AdminDayCalendar*/
    CLASS('AdminDayCalendar', EXTENDS(chlk.models.calendar.BaseCalendar), [


        chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsInputData',
        chlk.models.common.ChlkDate, 'date',
        Number, 'day',

        ArrayOf(chlk.models.period.Period), 'periods',

        [ria.serialize.SerializeProperty('gradelevels')],
        ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',

        [ria.serialize.SerializeProperty('calendardayitems')],
        ArrayOf(chlk.models.calendar.announcement.AdminDayCalendarItem), 'calendarDayItems',

        [[chlk.models.common.ChlkDate]],
        VOID, function setDate(date){
            this.date = date;
            this.setBaseCalendarData(date, null, null);
        },

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        OVERRIDE, VOID, function setBaseCalendarData(date, minDate, maxDate){
            BASE(date, minDate, maxDate);
            date = this.getCurrentDate();
            this.setPrevDate(date.add(chlk.models.common.ChlkDateEnum.DAY, -1));
            this.setNextDate(date.add(chlk.models.common.ChlkDateEnum.DAY, 1));
            this.setCurrentTitle(date.format('m/dd/yy'));
        }
    ]);
});