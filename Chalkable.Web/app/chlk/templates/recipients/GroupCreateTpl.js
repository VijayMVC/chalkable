REQUIRE('chlk.templates.controls.group_people_selector.SelectorBaseTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.GroupCreateTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/GroupCreateDialog.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupSelectorViewData)],
        'GroupCreateTpl', EXTENDS(chlk.templates.controls.group_people_selector.SelectorBaseTpl), [])
});