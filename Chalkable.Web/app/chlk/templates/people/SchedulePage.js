REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.people.SchedulePage');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.SchedulePage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/SchedulingPage.jade')],
        [ria.templates.ModelBind(chlk.models.people.SchedulePage)],
        'SchedulePage', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.Schedule, 'schedule',

            [ria.templates.ModelPropertyBind],
            chlk.models.calendar.announcement.Day, 'calendar'
        ])
});