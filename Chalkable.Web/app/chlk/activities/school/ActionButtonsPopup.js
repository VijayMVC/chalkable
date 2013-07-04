REQUIRE('chlk.activities.TemplatePopup');
REQUIRE('chlk.templates.school.ActionButtons');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.ActionButtonsPopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.IsHorizontalAxis(false)],
        [chlk.activities.isTopLeftPosition(false)],
        [chlk.activities.PageClass('profile')],
        [chlk.activities.BindTemplate(chlk.templates.school.ActionButtons)],
        'ActionButtonsPopup', EXTENDS(chlk.activities.TemplatePopup), [ ]);
});