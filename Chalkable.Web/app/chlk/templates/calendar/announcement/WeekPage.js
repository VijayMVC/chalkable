REQUIRE('chlk.templates.calendar.announcement.DayPage');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.templates.calendar.announcement.WeekCalendarBodyTpl');

NAMESPACE('chlk.templates.calendar.announcement', function () {
    "use strict";

    /** @class chlk.templates.calendar.announcement.WeekPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        [chlk.activities.lib.PageClass('calendar')],
        'WeekPage', EXTENDS(chlk.templates.calendar.announcement.DayPage), [
            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar'
        ]);
});