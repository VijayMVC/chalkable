REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.announcement.AnnouncementPeriod');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.WeekDayPopUp */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(true)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementPeriod)],
        'WeekDayPopUp', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});