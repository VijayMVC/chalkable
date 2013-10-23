REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileScheduleViewData');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.SchedulePage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/SchedulingPage.jade')],
        [ria.templates.ModelBind(chlk.models.people.UserProfileScheduleViewData)],
        'SchedulePage', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.people.Schedule)), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.Schedule, 'schedule',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.announcement.Day, 'calendar'
        ])
});