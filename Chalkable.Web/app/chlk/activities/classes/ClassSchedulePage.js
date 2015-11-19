REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassScheduleTpl');
REQUIRE('chlk.templates.calendar.announcement.DayPage');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassSchedulePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassScheduleTpl)],
        'ClassSchedulePage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});