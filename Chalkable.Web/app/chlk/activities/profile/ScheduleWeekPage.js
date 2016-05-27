REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.people.ScheduleWeekTpl');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.ScheduleWeekPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.people.ScheduleWeekTpl)],
        'ScheduleWeekPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});