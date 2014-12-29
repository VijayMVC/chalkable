REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.calendar.announcement.WeekDay');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.WeekBarPopUp */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.WeekDay)],
        'WeekBarPopUp', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});