REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.classes.StudentAlertsTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.StudentAlertsPopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(true)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.classes.StudentAlertsTpl)],
        'StudentAlertsPopup', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});