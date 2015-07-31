REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.announcement.WeekPage');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.WeekPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('calendar')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.WeekPage)],
        'WeekPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});