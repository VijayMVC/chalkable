REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileScheduleViewData');
REQUIRE('chlk.models.calendar.announcement.Week');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.ScheduleDayTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/SchedulingWeekPage.jade')],
        [ria.templates.ModelBind(chlk.models.people.UserProfileScheduleViewData.OF(chlk.models.calendar.announcement.Week))],
        'ScheduleDayTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.people.Schedule)), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.Schedule, 'schedule',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.announcement.Week, 'calendar'
        ])
});