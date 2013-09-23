REQUIRE('chlk.templates.calendar.BaseCalendarTpl');
REQUIRE('chlk.templates.calendar.announcement.AdminDayCalendarBodyTpl');
REQUIRE('chlk.models.calendar.announcement.AdminDayCalendar');

NAMESPACE('chlk.templates.calendar.announcement', function(){
    "use strict";

    /**@class chlk.templates.calendar.announcement.AdminDayCalendarTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/AdminDayCalendarPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.AdminDayCalendar)],
        'AdminDayCalendarTpl', EXTENDS(chlk.templates.calendar.BaseCalendarTpl),[

            [ria.templates.ModelPropertyBind()],
            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsInputData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.period.Period), 'periods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.AdminDayCalendarItem), 'calendarDayItems'


        ]);
});