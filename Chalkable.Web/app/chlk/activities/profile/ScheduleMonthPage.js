REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.people.ScheduleMonthTpl');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.ScheduleMonthPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.people.ScheduleMonthTpl)],
        'ScheduleMonthPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});