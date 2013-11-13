REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.people.ScheduleDayTpl');
REQUIRE('chlk.templates.calendar.announcement.DayPage');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.SchedulePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.people.ScheduleDayTpl)],
        'SchedulePage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});