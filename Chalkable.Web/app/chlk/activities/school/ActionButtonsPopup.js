REQUIRE('chlk.templates.school.ActionButtons');
REQUIRE('chlk.activities.lib.TemplatePopup');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.ActionButtonsPopup */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(false)],
        [ria.mvc.TemplateBind(chlk.templates.school.ActionButtons)],
        'ActionButtonsPopup', EXTENDS(chlk.activities.lib.TemplatePopup), [
            [ria.mvc.DomEventBind('click', '.action-link-button:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function handleClick(node, event) {
                var index = node.getData('index');
                var form = this.getDom().find('#emails-form');
                form.setFormValues({
                    index:index
                });
                form.triggerEvent('submit');
                return false;
            }
        ]);
});