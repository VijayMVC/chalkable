REQUIRE('chlk.templates.controls.group_people_selector.SelectorBaseTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.GroupSelectorTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/GroupSelectorDialog.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupSelectorViewData)],
        'GroupSelectorTpl', EXTENDS(chlk.templates.controls.group_people_selector.SelectorBaseTpl), [])
});