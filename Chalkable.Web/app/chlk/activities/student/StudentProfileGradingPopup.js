REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.student.StudentProfileGradingPopUpTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileGradingPopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileGradingPopUpTpl)],
        'StudentProfileGradingPopup', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});