REQUIRE('chlk.templates.school.ActionButtons');
REQUIRE('chlk.activities.lib.TemplatePopup');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.ActionButtonsPopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [chlk.activities.lib.PageClass('profile')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.ActionButtons)],
        'ActionButtonsPopup', EXTENDS(chlk.activities.lib.TemplatePopup), [ ]);
});