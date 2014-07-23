REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.calendar.announcement.DayDay');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.DayPeriodPopUp */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(true)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.DayDay)],
        'DayPeriodPopUp', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});