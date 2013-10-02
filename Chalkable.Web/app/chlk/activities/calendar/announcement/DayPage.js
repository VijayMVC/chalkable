REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.announcement.DayPage');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.DayPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('calendar')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.DayPage)],
        'DayPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});